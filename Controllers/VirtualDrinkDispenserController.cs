using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Virtual_Drinks_Dispenser.Model;

namespace Virtual_Drinks_Dispenser.Controllers
{
    
    [ApiController]
    [Route("api")]
    public class VirtualDrinkDispenserController : ControllerBase
    {
        private readonly object _lockObj = new object();
        private readonly int maxCokeOnServing = 2;
        private readonly int maxFantaOnServing = 1;
        private readonly ILogger<VirtualDrinkDispenserController> _logger;
        

        public VirtualDrinkDispenserController(ILogger<VirtualDrinkDispenserController> logger)
        {
            _logger = logger;
        }
        [Route("SetOrderPreparationTime")]
        [HttpPost]
        public ActionResult SetOrderPreparationTime(int preparationTimeInSec)
        {
            try
            {
                OrdersRepository.setPreparationTimeInSec(preparationTimeInSec);
                return StatusCode(200, "Preparation Time has been set!");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                throw ex;
            }

        }
        [Route("SetOrderResponseTime")]
        [HttpPost]
        public ActionResult SetOrderResponseTime(int orderResponseTimeInSec)
        {
            try
            {
                OrdersRepository.setOrderResponseTimeInSec(orderResponseTimeInSec);
                return StatusCode(200, "Order Response Time has been set!");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                throw;
            }

        }
        [Route("OrderDrink")]
        [HttpPost]
        public async Task<ActionResult> OrderDrink([Required]string customerNumber, [Required] DrinkType drinkType)
        {
            try
            {
                var OrderDetail = new OrderModel(customerNumber, drinkType);
                await Task.Delay(OrdersRepository.orderResponseTimeInSec * 1000);
                lock (_lockObj)
                {
                    if (checkOrderAcceptance(OrderDetail))
                    {
                        PrepareAndServeDrink(OrderDetail);
                        return StatusCode(200, "Order has been placed successfully");
                    }
                    else
                    {
                        _logger.LogInformation("429 code return");
                        return StatusCode(429, "Sorry! Cannot Accept further orders");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                throw;
            }
            
        }
        [Route("AllOrders")]
        [HttpGet]
        public ActionResult<IEnumerable<OrderModel>> GetAllOrdersList()
        {
            try
            {
                _logger.LogInformation("Getting list of All Orders");
                return OrdersRepository.AllOrders.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                throw;
            }
            
        }
        [NonAction]
        public bool checkOrderAcceptance(OrderModel order)
        {
            _logger.LogInformation("Checking the order acceptance");
            if (order.drinkType == DrinkType.Coke)
            {
                return OrdersRepository.totalLoad < maxCokeOnServing;
            }
            else
            {
                return OrdersRepository.totalLoad < maxFantaOnServing;
            }
        }
        [NonAction]
        private async void PrepareAndServeDrink(OrderModel order)
        {
            _logger.LogInformation("Order Created for drink: "+ order.drinkType);
            OrdersRepository.Create(order);
            if (order.drinkType== DrinkType.Coke)
            {
                OrdersRepository.totalLoad += 1;
            }
            else if (order.drinkType == DrinkType.Fanta)
            {
                OrdersRepository.totalLoad += 2;
            }
            _logger.LogInformation("Waiting for Order Preparation");
            await Task.Delay(OrdersRepository.preparationTimeInSec * 1000);
            _logger.LogInformation("Serving the Order for customer number: "+ order.customerNumber);
            lock (_lockObj)
            {
                var getOrderInfo = OrdersRepository.AllOrders.Where(x => x.Id == order.Id).FirstOrDefault();
                if (getOrderInfo!=null)
                getOrderInfo.OrderState = OrderState.Served;
                if (order.drinkType == DrinkType.Coke)
                {
                    OrdersRepository.totalLoad -= 1;
                }
                else if (order.drinkType == DrinkType.Fanta)
                {
                    OrdersRepository.totalLoad -= 2;
                }
            }
        }
    }
}
