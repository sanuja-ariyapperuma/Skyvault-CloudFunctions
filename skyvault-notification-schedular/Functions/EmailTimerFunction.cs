using System;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Tls;
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
                var tasks = new[]
                {
                    SendBirthdayNotification(),
                    //SendPassportExpirationNotification(connectionString),
                    //SendVisaExpirationNotification(connectionString)
                };

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                LoggerService.Log.LogError($"Unexpected error occurred: {ex.Message}");
            }
        }

        private async Task SendBirthdayNotification()
        {
            var recipients = await customerRepository.GetCustomersWithBirthdayToday();
            var birthdayImageURL = await templateRepository.GetBirthdayImageURL();

            if (recipients == null || !recipients.Any())
            {
                LoggerService.Log.LogInformation("No birthday notifications to send today.");
                return;
            }

            if (string.IsNullOrEmpty(birthdayImageURL))
            {
                LoggerService.Log.LogError("No birthday image URL found.");
                return;
            }

            LoggerService.Log.LogInformation($"Sending birthday notifications to : {recipients.Count()} clients");
            LoggerService.Log.LogInformation($"Image URL : {birthdayImageURL}");

            recipients.ForEach(recipient => recipient.SetEmailBody(birthdayImageURL));

            await emailService.SendEmailAsync(recipients, "Greetings from Travel Channel (Private) Limited");
        }


        //private async Task SendPassportExpirationNotification(string connectionString)
        //{
        //    await using var connection = new MySqlConnection(connectionString);
        //    await connection.OpenAsync();

        //    string sixMonthsFromNow = DateTime.UtcNow.AddMonths(6).ToString("yyyy-MM-dd");
        //    using var cmd = new MySqlCommand($"SELECT COUNT(*) FROM passports WHERE ExpiryDate = '{sixMonthsFromNow}'", connection);
        //    int count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        //    _logger.LogInformation($"Passports expiring in 6 months count: {count}");
        //}

        //private async Task SendVisaExpirationNotification(string connectionString)
        //{
        //    await using var connection = new MySqlConnection(connectionString);
        //    await connection.OpenAsync();

        //    using var cmd = new MySqlCommand("SELECT COUNT(*) FROM visas WHERE expire_date < NOW()", connection);
        //    int count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        //    _logger.LogInformation($"Visa expiration count: {count}");
        //}
    }
}
