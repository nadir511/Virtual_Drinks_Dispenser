using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
    [JsonConverter(typeof(JsonStringEnumConverter))]

    public enum DrinkType
    {
        Coke,
        Fanta
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]

    public enum OrderState
    {
        Serving,
        Served
    }

}
