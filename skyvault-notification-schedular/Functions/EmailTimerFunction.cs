using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using skyvault_notification_schedular.Data;
using skyvault_notification_schedular.Services;
using skyvault_notification_schedular.Helpers;
using skyvault_notification_schedular.Models;

namespace skyvault_notification_schedular.Functions
{
    public class EmailTimerFunction(
            ILoggerFactory loggerFactory,
            ICustomerRepository customerRepository,
            ITemplateRepository templateRepository,
            IEmailService emailService)
    {

        [Function("EmailTimerFunction")]
        public async Task RunAsync([TimerTrigger("0 0 0 * * *")] TimerInfo myTimer)
        {
            LoggerService.Initialize(loggerFactory);

            try
            {
                var tasks = new List<Task>
                    {
                        SendBirthdayNotification(),
                        SendPassportExpirationNotification(),
                        SendVisaExpirationNotification()
                    };

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                LoggerService.Log.LogError(ex, "Unexpected error occurred: {Message}", ex.Message);
            }
        }

        private async Task SendBirthdayNotification()
        {
            var recipients = await customerRepository.GetCustomersWithBirthdayToday();
            var birthdayImageURL = (await templateRepository.GetEmailContent(NotificationTypeEnum.Birthday))?.File;

            if (recipients == null || recipients.Count == 0)
            {
                LoggerService.Log.LogInformation("No birthday notifications to send today.");
                return;
            }

            if (string.IsNullOrEmpty(birthdayImageURL))
            {
                LoggerService.Log.LogError("No birthday image URL found.");
                return;
            }

            var fullPath = FileHelpers.GetFileFullPath(birthdayImageURL, NotificationTypeEnum.Birthday);

            LoggerService.Log.LogInformation("Sending birthday notifications to : {Count} clients", recipients.Count);

            recipients.ForEach(recipient => recipient.SetBirthdayEmailBody(fullPath));

            await emailService.SendEmailAsync(recipients, "Greetings from Travel Channel (Private) Limited");
        }

        private async Task SendPassportExpirationNotification()
        {
            string sixMonthsFromNow = DateTime.UtcNow.AddMonths(6).ToString("yyyy-MM-dd");
            var recipients = await customerRepository.GetCustomersWithPassportExpiryFromSixMonths(sixMonthsFromNow);

            if (recipients == null || recipients.Count == 0)
            {
                LoggerService.Log.LogInformation("No passport expiry notifications to send today.");
                return;
            }

            var messageContent = await templateRepository.GetEmailContent(NotificationTypeEnum.PassportExpiration);
            var message = messageContent?.Content;

            if (string.IsNullOrWhiteSpace(message) || !message.Contains('|'))
            {
                LoggerService.Log.LogError("Invalid or missing passport expiry message.");
                return;
            }

            if (!message.Contains("passport_number"))
            {
                LoggerService.Log.LogError("No passport_number found in passport expiry message.");
                return;
            }

            var (subject, body) = SplitMessage(message);
            LoggerService.Log.LogInformation("Sending passport expiry notifications to: {Count} clients", recipients.Count);

            recipients.ForEach(r => {
                r.SetPassportOrVisaEmailBody(body);
                r.EmailBody = r.EmailBody.Replace("passport_number", r.PassportNumber);
            }
            );
            await emailService.SendEmailAsync(recipients, subject);

        }

        private static (string Subject, string Body) SplitMessage(string message)
        {
            var parts = message.Split('|', 2); // limit split to 2 parts
            return (parts[0], parts.Length > 1 ? parts[1] : string.Empty);
        }

        private async Task SendVisaExpirationNotification()
        {
            string threeMonthsFromNow = DateTime.UtcNow.AddMonths(3).ToString("yyyy-MM-dd");
            var recipients = await customerRepository.GetCustomersWithVisaExpiryFromThreeMonths(threeMonthsFromNow);

            if (recipients == null || recipients.Count == 0)
            {
                LoggerService.Log.LogInformation("No VISA expiry notifications to send today.");
                return;
            }

            var messageContent = await templateRepository.GetEmailContent(NotificationTypeEnum.VisaExpiration);
            var message = messageContent?.Content;

            if (string.IsNullOrWhiteSpace(message) || !message.Contains('|'))
            {
                LoggerService.Log.LogError("Invalid or missing visa expiry message.");
                return;
            }

            if (!message.Contains("country_name"))
            {
                LoggerService.Log.LogError("No country_name found in visa expiry message.");
                return;
            }

            var (subject, body) = SplitMessage(message);
            LoggerService.Log.LogInformation("Sending passport expiry notifications to: {Count} clients", recipients.Count);

            recipients.ForEach(r => {
                r.SetPassportOrVisaEmailBody(body);
                r.EmailBody = r.EmailBody.Replace("country_name", r.VisaCountry);
                }
            );

            await emailService.SendEmailAsync(recipients, subject);
        }
    }
}
