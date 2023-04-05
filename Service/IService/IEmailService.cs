using IotSupplyStore.Models.DtoModel;

namespace IotSupplyStore.Service.IService
{
    public interface IMailService
    {
        Task<bool> SendAsync(MailData mailData, CancellationToken ct);
    }
}
