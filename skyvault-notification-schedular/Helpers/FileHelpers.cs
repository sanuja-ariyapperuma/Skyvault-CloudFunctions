using skyvault_notification_schedular.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skyvault_notification_schedular.Helpers
{
    public static class FileHelpers
    {
        public static string GetFileFullPath(string fileName, NotificationTypeEnum notificationType)
        {
            var blobStorageName = Environment.GetEnvironmentVariable("BLOB_STORAGE_NAME");
            var fileFullPath = String.Concat("https://", blobStorageName, ".blob.core.windows.net/", blobStorageName, "/");

            switch (notificationType)
            {
                case NotificationTypeEnum.Birthday:
                    return String.Concat(fileFullPath, "birthdaywishes/", fileName);
                case NotificationTypeEnum.Custom:
                    return String.Concat(fileFullPath, "promotions/", fileName);
                default:
                    throw new ArgumentException("Invalid notification type");
            }

        }
    }
}
