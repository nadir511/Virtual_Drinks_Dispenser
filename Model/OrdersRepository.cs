namespace Virtual_Drinks_Dispenser.Model
{
    public static class OrdersRepository
    {
        public static int totalLoad { get; set; } = 0;
        public static int preparationTimeInSec = 5;
        public static int orderResponseTimeInSec = 0;
        private static readonly List<OrderModel> ordersObj = new List<OrderModel>();
        public static IEnumerable<OrderModel> AllOrders
        {
            get { return ordersObj; }
        }
        public static void Create(OrderModel _orders)
        {
            ordersObj.Add(_orders);
        }
        public static void setPreparationTimeInSec(int preparationTime)
        {
          preparationTimeInSec = preparationTime;
        }
        public static void setOrderResponseTimeInSec(int responseTime)
        {
            orderResponseTimeInSec = responseTime;
        }
    }
}
