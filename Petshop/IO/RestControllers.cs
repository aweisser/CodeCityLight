using Petshop.Ordering;

namespace Petshop.Api.Rest
{
    public class OrderRestController
    {
        private OrderManagement _orderManagement;

        public OrderRestController(OrderManagement orderManagement)
        {
            _orderManagement = orderManagement;
        }

        public void PostOrder()
        {
            Order order = getOrderFromHttpRequest();
            _orderManagement.Apply(order);
        }

        private Order getOrderFromHttpRequest()
        {
            return new Order();
        }
    }
}
