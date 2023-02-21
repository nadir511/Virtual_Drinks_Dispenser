using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Virtual_Drinks_Dispenser.Model;

namespace Virtual_Drinks_Dispenser.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VirtualDrinkDispenserController : ControllerBase
    {
        private readonly object _lockObj = new object();
        private readonly List<OrderModel> orders = new List<OrderModel>();
        private readonly int cokesCapacity = 2;
        private readonly int fantaCpacity = 1;
        private readonly int preparationTimeInSec = 10;
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
                    OrderPreparation(OrderDetail);
                    return StatusCode(200);
                }
                else
                {
                    return StatusCode(429);
                }
            }
        }
        [HttpGet]
        public ActionResult<IEnumerable<OrderModel>> GetAllOrdersList()
        {
            return orders.ToList();
        }
        public bool checkOrderAcceptance(OrderModel order)
        {
            if (order.drinkType == DrinkType.Coke)
            {
                return orders.Where(x=>x.drinkType== DrinkType.Coke && x.OrderState == OrderState.Serving).ToList().Count< cokesCapacity;
            }
            else
            {
                return orders.Where(x => x.drinkType ==DrinkType.Fanta && x.OrderState == OrderState.Serving).ToList().Count < fantaCpacity;
            }
        }
        private void OrderPreparation(OrderModel order)
        {
            orders.Add(order);
            ThreadPool.QueueUserWorkItem(_ => ServeDrink(order));
        }
        private void ServeDrink(OrderModel order)
        {
            Thread.Sleep(preparationTimeInSec * 1000);
            lock (_lockObj)
            {
                var getOrderInfo = orders.Where(x => x.Id == order.Id).FirstOrDefault();
                if (getOrderInfo!=null)
                getOrderInfo.OrderState = OrderState.Served;
            }
        }
    }
}
