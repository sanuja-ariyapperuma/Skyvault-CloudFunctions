using brevo_csharp.Api;
using brevo_csharp.Client;
using brevo_csharp.Model;
using Microsoft.Extensions.Logging;
using skyvault_notification_schedular.Data;
using skyvault_notification_schedular.Models;

namespace skyvault_notification_schedular.Services
{
    public class EmailServiceBrevo : IEmailService
    {
        private readonly ILogger<EmailServiceBrevo> _logger;
        private static readonly string BrevoApiKey = Environment.GetEnvironmentVariable("BREVO_API_KEY") ?? throw new InvalidOperationException("Brevo API key not found in environment variables.");
        private static readonly int BatchSize = 50;
        private static readonly string SenderEmail = "a.sanuja.r@gmail.com";
        private static readonly string SenderName = "Travel Channel (Private) Limited";
        private static readonly int DelayBetweenBatches = 2000;

        public EmailServiceBrevo(ILogger<EmailServiceBrevo> logger)
        {
            _logger = logger;
            Configuration.Default.ApiKey.Add("api-key", BrevoApiKey);


        }

        public async System.Threading.Tasks.Task SendEmailAsync(List<Recipient> recipients, string subject)
        {


            int totalRecipientCount = recipients.Count;

            for (int i = 0; i < totalRecipientCount; i += BatchSize)
            {
                List<Recipient> batch = recipients.GetRange(i, Math.Min(BatchSize, totalRecipientCount - i));

                await SendBatchAsync(batch, subject, i / BatchSize + 1);

                _logger.LogInformation("Emails {sent}/{total} sent successfully.", (batch.Count * (i + 1)), totalRecipientCount);

                if (i + BatchSize < totalRecipientCount)
                {
                    await System.Threading.Tasks.Task.Delay(DelayBetweenBatches);
                }
            }
        }
        private async System.Threading.Tasks.Task SendBatchAsync(List<Recipient> recipients, string subject, int batchNumber)
        {
            var sender = new SendSmtpEmailSender
            {
                Email = SenderEmail,
                Name = SenderName
            };

            var messageVersions = GenerateMessageVersions(recipients);

            var sendSmtpEmail = new SendSmtpEmail
            {
                Sender = sender,
                Subject = subject,
                HtmlContent = "<html><body><p>This is a placeholder content.</p></body></html>", // This is a placeholder content. Function does not work if this absent
                MessageVersions = messageVersions
            };

            TransactionalEmailsApi transactionalEmailsApi = new TransactionalEmailsApi();
            var result = await transactionalEmailsApi.SendTransacEmailAsync(sendSmtpEmail);

            _logger.LogInformation("Email sent successfully. Result: {result}", result);

            //Check if the email was sent successfully
            if (result == null || result.MessageId == null)
            {
                _logger.LogError("Email sending failed for batch {batchNumber}.", batchNumber);
            }
        }

        private static List<SendSmtpEmailMessageVersions> GenerateMessageVersions(List<Recipient> recipients)
        {
            var messageVersions = new List<SendSmtpEmailMessageVersions>(recipients.Count);

            foreach (var recipient in recipients)
            {
                var messageVersion = new SendSmtpEmailMessageVersions([new SendSmtpEmailTo1(recipient.Email, recipient.Name)])
                {
                    HtmlContent = recipient.EmailBody
                };
                messageVersions.Add(messageVersion);
            }
            return messageVersions;
        }

        public async Task<BrevoAccountInformation> GetAccountInfomationAsync()
        {
            var accountApi = new AccountApi();
            GetAccount response = await accountApi.GetAccountAsync();

            var accountInformation = new BrevoAccountInformation
            {
                CompanyName = response.CompanyName,
                CompanyEmailAddress = response.Email,
                Plans = response.Plan
                    .Where(plan => plan.Type == GetAccountPlan.TypeEnum.Free || plan.Type == GetAccountPlan.TypeEnum.PayAsYouGo)
                    .Select(plan => new BrevoPlan
                    {
                        Type = plan.Type.ToString(),
                        CreditsType = plan.CreditsType.ToString(),
                        Credits = plan.Credits
                    }).ToList()
            };

            return accountInformation;
        }
    }
}
