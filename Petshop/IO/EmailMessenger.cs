using Petshop.Shipping;

namespace Petshop.Messaging.Email
{
    class EmailMessenger : IShippingMessenger
    {
        public void SendShippingConfirmation(string message)
        {
            // send confirmation using Email ...
        }
    }
}
