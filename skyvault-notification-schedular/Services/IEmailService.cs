using skyvault_notification_schedular.Data;
using skyvault_notification_schedular.Models;

namespace skyvault_notification_schedular.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(List<Recipient> recipients, string subject);
        Task<BrevoAccountInformation> GetAccountInfomationAsync();
    }
}
