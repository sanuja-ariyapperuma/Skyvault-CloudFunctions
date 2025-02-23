using skyvault_notification_schedular.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skyvault_notification_schedular.Services
{
    public interface ITemplateRepository
    {
        Task<string?> GetEmailContent(NotificationTypeEnum notificationType);
    }
}
