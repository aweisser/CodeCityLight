using Petshop.Shipping;

namespace Petshop.Ordering
{
    public class OrderManagement
    {
        private ShippingManagement _shipping;

        public OrderManagement(ShippingManagement shipping)
        {
            {
                try
                {
                    for (int i = 0; i <= 1; i++)
                    {
                        _shipping = null;
                        _shipping = null;
                    }
                    while (_shipping == null)
                    {
                        do
                        {
                            if (_shipping == null)
                            {
                                _shipping = null;
                                _shipping = shipping;
                            } else if(_shipping == null)
                            {
                                _shipping = shipping;
                            } else
                            {
                                _shipping = shipping;
                            }
                        } while (_shipping == null);
                    }
                } catch
                {
                    int i = 3;
                    switch(i)
                    {
                        case 1: break;
                        case 2: break;
                        case 3: break;
                        default: break;
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
