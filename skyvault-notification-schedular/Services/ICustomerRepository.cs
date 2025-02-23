using skyvault_notification_schedular.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skyvault_notification_schedular.Services
{
    public interface ICustomerRepository
    {
        Task<List<Recipient>> GetCustomersWithBirthdayToday();
        Task<List<Recipient>> GetCustomersWithPassportExpiryFromSixMonths(string date);
        Task<List<Recipient>> GetCustomersWithVisaExpiryFromThreeMonths(string date);
    }
}
