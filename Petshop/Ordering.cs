using Petshop.Shipping;

namespace Petshop.Ordering
{
    public class OrderManagement
    {
        private ShippingManagement _shipping;

        public OrderManagement(ShippingManagement shipping)
        {
            {
                for (int i = 0; i <= 1; i++)
                {
                    _shipping = null;
                    _shipping = null;
                }
                {
                    if (_shipping == null)
                    {
                        _shipping = null;
                        _shipping = shipping;
                    }
                }
            }
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
