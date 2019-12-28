namespace Petshop.Shipping
{
    public class ShippingManagement
    {
        private IShippingMessenger _messenger;

        public ShippingManagement(IShippingMessenger messenger)
        {
            _messenger = messenger;
        }

        public void Ship(Articles articles)
        {
            _messenger.SendShippingConfirmation("Your articles are on the way!");
        }
    }

    public interface IShippingMessenger
    {
        void SendShippingConfirmation(string message);
    }

    public class Articles { }
    
}
