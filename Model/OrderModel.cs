namespace Virtual_Drinks_Dispenser.Model
{
    public class OrderModel
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        public string? customerNumber { get; }
        public DrinkTypeModel DrinkTypeModel { get; }
        public OrderStateModel OrderStateModel { get; }
        public DrinkType drinkType { get; }
        public OrderState OrderState { get; set; } = OrderState.Serving;
        public OrderModel(string CustomerNumber, DrinkType DrinkType)
        {
            customerNumber = CustomerNumber;
            drinkType = DrinkType;
        }
    }
    public class OrderStateModel
    {
        public string? Serving { get; }
        public string? Served { get; }
    }
    public class DrinkTypeModel
    {
        public string? Coke { get; }
        public string? Fanta { get; }
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
