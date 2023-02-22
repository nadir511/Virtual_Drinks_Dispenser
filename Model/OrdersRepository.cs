namespace Virtual_Drinks_Dispenser.Model
{
    public static class OrdersRepository
    {
        private static readonly List<OrderModel> ordersObj = new List<OrderModel>();
        public static IEnumerable<OrderModel> AllOrders
        {
            get { return ordersObj; }
        }
        public static void Create(OrderModel _orders)
        {
            ordersObj.Add(_orders);
        }
    }
}
