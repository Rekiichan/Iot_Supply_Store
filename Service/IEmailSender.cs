using IotSupplyStore.Models;

namespace IotSupplyStore.Service
{
    public interface IEmailSender
    {
        void SendEmail(Message message);
    }
}
