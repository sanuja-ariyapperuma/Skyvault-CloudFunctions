using skyvault_notification_schedular.Data;
using skyvault_notification_schedular.Models;

namespace skyvault_notification_schedular.Services
{
    public interface ICustomerRepository
    {
        Task<List<Recipient>> GetCustomersWithBirthdayToday();
        Task<List<Recipient>> GetCustomersWithPassportExpiryFromSixMonths(string date);
        Task<List<Recipient>> GetCustomersWithVisaExpiryFromThreeMonths(string date);
        Task<List<Recipient>> GetCustomersForPromotion(CommiunicationMethodEnum commiunicationMethod);
    }
}
