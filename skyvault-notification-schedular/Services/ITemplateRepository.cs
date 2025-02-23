using skyvault_notification_schedular.Models;

namespace skyvault_notification_schedular.Services
{
    public interface ITemplateRepository
    {
        Task<string?> GetEmailContent(NotificationTypeEnum notificationType);
    }
}
