using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Virtual_Drinks_Dispenser.Model
{
    public class OrderModel
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        
        public string? customerNumber { get; }
        
        public DrinkType drinkType { get; }
        public OrderState OrderState { get; set; } = OrderState.Serving;
        public OrderModel(string CustomerNumber, DrinkType DrinkType)
        {
            customerNumber = CustomerNumber;
            drinkType = DrinkType;
        }
    }
    public enum DrinkType
    {
        Coke,
        
        Fanta
    }
    public enum OrderState
    {
        Serving,
        Served
    }

}
