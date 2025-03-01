using skyvault_notification_schedular.Data;
using skyvault_notification_schedular.Models;

namespace skyvault_notification_schedular.Services
{
    public sealed class CustomerRepository(IDataAccess dataAccess) : ICustomerRepository
    {
        private readonly string _commonSQL = @"
                SELECT CONCAT(s.salutation_name, '. ', p.other_names, ' ', p.last_name) AS name, 
                       cp.email,
                       c.country_name AS VisaCountry
                FROM customer_profiles cp 
                INNER JOIN passports p ON cp.id = p.customer_profile_id 
                INNER JOIN salutations s ON cp.salutation_id = s.id 
                LEFT JOIN visas vsa ON p.id = vsa.passport_id
                INNER JOIN countries c ON vsa.DestinationCountryId = c.id
                WHERE cp.email != ''
            ";

        private readonly IDataAccess _dataAccess = dataAccess;

        public async Task<List<Recipient>> GetCustomersForPromotion(CommiunicationMethodEnum commiunicationMethod)
        {
            string sql = @"SELECT CONCAT(s.salutation_name, '. ', p.other_names, ' ', p.last_name) AS name,
                        cp.email
                        FROM customer_profiles cp
                        INNER JOIN passports p ON cp.id = p.customer_profile_id
                        INNER JOIN salutations s ON cp.salutation_id = s.id
                        WHERE preferred_comm_id = {0}
                        GROUP BY cp.email, s.salutation_name, p.other_names, p.last_name";

            sql = string.Format(sql, (int)commiunicationMethod);

            var result = await _dataAccess.QueryAsync<Recipient>(sql);

            return [.. result];
        }

        public async Task<List<Recipient>> GetCustomersWithBirthdayToday()
        {


            string sql = $@"
                  {_commonSQL}
                  AND p.is_primary = 1 
                  AND DATE_FORMAT(p.date_of_birth, '%m-%d') = DATE_FORMAT(CURDATE(), '%m-%d')";

            var result = await _dataAccess.QueryAsync<Recipient>(sql);

            return [.. result];
        }

        public async Task<List<Recipient>> GetCustomersWithPassportExpiryFromSixMonths(string date)
        {


            string sql = $@"
                  {_commonSQL}
                  AND p.ExpiryDate = '{date}'";

            var result = await _dataAccess.QueryAsync<Recipient>(sql);
            return [.. result];

        }

        public async Task<List<Recipient>> GetCustomersWithVisaExpiryFromThreeMonths(string date)
        {


            string sql = $@"
                  {_commonSQL}
                  AND vsa.expire_date = '{date}'";

            var result = await _dataAccess.QueryAsync<Recipient>(sql);
            return [.. result];
        }
    }
}
