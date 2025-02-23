using Microsoft.Extensions.Logging;
using skyvault_notification_schedular.Models;
using System.Text;
using System.Text.Json;

namespace skyvault_notification_schedular.Services
{
    public class BrevoEmailService : IEmailService
    {
        private static readonly HttpClient httpClient = new();
        private static readonly string BrevoApiKey = Environment.GetEnvironmentVariable("BREVO_API_KEY") ?? throw new InvalidOperationException("Brevo API key not found in environment variables.");
        private static readonly int BatchSize = 10;
        private static readonly string SenderEmail = "a.sanuja.r@gmail.com";
        private static readonly string SenderName = "Travel Channel (Private) Limited";

        public async Task SendEmailAsync(List<Recipient> recipients, string subject)
        {
            for (int i = 0; i < recipients.Count; i += BatchSize)
            {
                var batch = recipients.GetRange(i, Math.Min(BatchSize, recipients.Count - i));
                var toList = new List<object>();

                foreach (var recipient in batch)
                {
                    toList.Add(new
                    {
                        email = recipient.Email,
                        name = recipient.Name
                    });
                }

                var payload = new
                {
                    to = toList,
                    sender = new
                    {
                        email = SenderEmail,
                        name = SenderName
                    },
                    subject,
                    htmlContent = string.Join("<br>", batch.Select(r => r.EmailBody))
                };

                var jsonPayload = JsonSerializer.Serialize(payload);
                var requestContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("api-key", BrevoApiKey);

                var response = await httpClient.PostAsync("https://api.brevo.com/v3/smtp/email", requestContent);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    LoggerService.Log.LogInformation($"Batch {i / BatchSize + 1} sent successfully.");
                }
                else
                {
                    LoggerService.Log.LogError($"Failed to send batch {i / BatchSize + 1}. Response: {responseBody}");
                }
            }
        }
    }
}
