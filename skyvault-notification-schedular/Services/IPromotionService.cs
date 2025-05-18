using skyvault_notification_schedular.Models;

namespace skyvault_notification_schedular.Services
{
    public interface IPromotionService
    {
        Task SendPromotionAsync(string template, List<Recipient> users);
    }
}
