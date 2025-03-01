using Dapper;
using MySql.Data.MySqlClient;
using skyvault_notification_schedular.Data;

namespace skyvault_notification_schedular.Services
{
    public sealed class TemplateRepository(string connectionString) : ITemplateRepository
    {
        public async Task<EmailContent?> GetEmailContent(NotificationTypeEnum notificationType)
        {
            string query = "SELECT {0} FROM notification_templates WHERE Active = 1 AND notification_type = {1}";

            switch (notificationType)
            {
                case NotificationTypeEnum.Birthday:
                    query = string.Format(query, "file", 1);
                    return await GetTemplateContentAsync(query);
                case NotificationTypeEnum.PassportExpiration:
                    query = string.Format(query, "content", 2);
                    return await GetTemplateContentAsync(query);
                case NotificationTypeEnum.VisaExpiration:
                    query = string.Format(query, "content", 3);
                    return await GetTemplateContentAsync(query);
                default:
                    return null;
            }
        }

        public Task<EmailContent?> GetPromotionContent(int templateId)
        {
            string query = "SELECT file, content FROM notification_templates WHERE id={0} AND notification_type = 4";
            query = string.Format(query, templateId);
            return GetTemplateContentAsync(query);
        }

        private async Task<EmailContent?> GetTemplateContentAsync(string query)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            return await connection.QueryFirstOrDefaultAsync<EmailContent>(query);
        }
    }
}
