using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Virtual_Drinks_Dispenser.Model;

namespace Virtual_Drinks_Dispenser.Controllers
{
    
    [ApiController]
    [Route("api")]
    public class VirtualDrinkDispenserController : ControllerBase
    {
        private readonly object _lockObj = new object();
        //private readonly List<OrderModel> orders = new List<OrderModel>();
        private readonly int cokesCapacity = 2;
        private readonly int fantaCpacity = 1;
        private readonly int preparationTimeInSec = 20;
        public VirtualDrinkDispenserController()
        {

        }
        [Route("OrderDrink")]
        [HttpPost]
        public ActionResult OrderDrink(string customerNumber,DrinkType drinkType)
        {
            var OrderDetail = new OrderModel(customerNumber,drinkType);

            lock (_lockObj)
            {
                if (checkOrderAcceptance(OrderDetail))
                {
                    PrepareAndServeDrink(OrderDetail);
                    return StatusCode(200,"Order has been placed successfully");
                }
                else
                {
                    return StatusCode(429,"Sorry! Cannot Accept further orders");
                }
            }
        }
        [Route("AllOrders")]
        [HttpGet]
        public ActionResult<IEnumerable<OrderModel>> GetAllOrdersList()
        {
            return OrdersRepository.AllOrders.ToList();
        }
        [NonAction]
        public bool checkOrderAcceptance(OrderModel order)
        {
            if (order.drinkType == DrinkType.Coke)
            {
                return OrdersRepository.AllOrders.Where(x=>x.drinkType== DrinkType.Coke && x.OrderState == OrderState.Serving).ToList().Count< cokesCapacity;
            }
            else
            {
                return OrdersRepository.AllOrders.Where(x => x.drinkType ==DrinkType.Fanta && x.OrderState == OrderState.Serving).ToList().Count < fantaCpacity;
            }
        }
        [NonAction]
        private async void PrepareAndServeDrink(OrderModel order)
        {
            OrdersRepository.Create(order);
            await Task.Delay(preparationTimeInSec * 1000);
            lock (_lockObj)
            {
                var getOrderInfo = OrdersRepository.AllOrders.Where(x => x.Id == order.Id).FirstOrDefault();
                if (getOrderInfo!=null)
                getOrderInfo.OrderState = OrderState.Served;
            }
        }
    }
}
