using skyvault_notification_schedular.Data;

namespace skyvault_notification_schedular.Services
{
    public interface ITemplateRepository
    {
        Task<EmailContent?> GetEmailContent(NotificationTypeEnum notificationType);
        Task<EmailContent?> GetPromotionContent(int templateId);
    }
}
