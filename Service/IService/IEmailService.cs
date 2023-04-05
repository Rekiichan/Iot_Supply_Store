using IotSupplyStore.Models.DtoModel;

namespace IotSupplyStore.Service.IService
{
    public interface IMailService
    {
        Task<bool> SendAsync(EmailDto mailData, CancellationToken ct);
    }
}
