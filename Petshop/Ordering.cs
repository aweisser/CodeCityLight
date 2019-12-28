using Petshop.Shipping;

namespace Petshop.Ordering
{
    public class OrderManagement
    {
        private ShippingManagement _shipping;

        public OrderManagement(ShippingManagement shipping)
        {
            _shipping = shipping;
        }

        public void Apply(Order order)
        {
            _shipping.Ship(order.Articles);
        }
    }

    public class Order
    {
        public Articles Articles { get; }
    }
}
