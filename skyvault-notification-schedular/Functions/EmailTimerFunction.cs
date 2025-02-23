using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using skyvault_notification_schedular.Models;
using skyvault_notification_schedular.Services;

namespace skyvault_notification_schedular.Functions
{
    public class EmailTimerFunction(
            ILoggerFactory loggerFactory,
            ICustomerRepository customerRepository,
            ITemplateRepository templateRepository,
            IEmailService emailService)
    {

        [Function("EmailTimerFunctionr")]
        public async Task RunAsync([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer)
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
            var birthdayImageURL = await templateRepository.GetEmailContent(NotificationTypeEnum.Birthday);

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

            LoggerService.Log.LogInformation("Sending birthday notifications to : {Count} clients", recipients.Count());

            recipients.ForEach(recipient => recipient.SetBirthdayEmailBody(birthdayImageURL));

            await emailService.SendEmailAsync(recipients, "Greetings from Travel Channel (Private) Limited");
        }


        private async Task SendPassportExpirationNotification()
        {
            string sixMonthsFromNow = DateTime.UtcNow.AddMonths(6).ToString("yyyy-MM-dd");
            var recipients = await customerRepository.GetCustomersWithPassportExpiryFromSixMonths(sixMonthsFromNow);
            var message = await templateRepository.GetEmailContent(NotificationTypeEnum.PassportExpiration);

            if (recipients == null || recipients.Count == 0)
            {
                LoggerService.Log.LogInformation("No passport expiry notifications to send today.");
                return;
            }

            if (string.IsNullOrEmpty(message))
            {
                LoggerService.Log.LogError("No passport expiry message found.");
                return;
            }

            LoggerService.Log.LogInformation("Sending passport expiry notifications to : {Count} clients", recipients.Count());

            recipients.ForEach(recipient => recipient.SetPassportOrVisaEmailBody(message));

            await emailService.SendEmailAsync(recipients, "Passport Expiry Reminder - Travel Channel (Private) Limited");

        }

        private async Task SendVisaExpirationNotification()
        {
            string threeMonthsFromNow = DateTime.UtcNow.AddMonths(3).ToString("yyyy-MM-dd");
            var recipients = await customerRepository.GetCustomersWithVisaExpiryFromThreeMonths(threeMonthsFromNow);
            var message = await templateRepository.GetEmailContent(NotificationTypeEnum.VisaExpiration);

            if (recipients == null || recipients.Count == 0)
            {
                LoggerService.Log.LogInformation("No visa expiry notifications to send today.");
                return;
            }

            if (string.IsNullOrEmpty(message))
            {
                LoggerService.Log.LogError("No visa expiry message found.");
                return;
            }

            LoggerService.Log.LogInformation("Sending visa expiry notifications to : {Count} clients", recipients.Count());

            recipients.ForEach(recipient => recipient.SetPassportOrVisaEmailBody(message));

            await emailService.SendEmailAsync(recipients, "Visa Expiry Reminder - Travel Channel (Private) Limited");

        }
    }
}
