using Dapper;
using MySql.Data.MySqlClient;
using skyvault_notification_schedular.Models;

namespace skyvault_notification_schedular.Services
{
    public sealed class CustomerRepository(string connectionString) : ICustomerRepository
    {
        private readonly string _commonSQL = @"
                SELECT CONCAT(s.salutation_name, '. ', other_names, ' ', last_name) AS name, 
                       cp.email 
                FROM customer_profiles cp 
                INNER JOIN passports p ON cp.id = p.customer_profile_id 
                INNER JOIN salutations s ON cp.salutation_id = s.id 
                LEFT JOIN visas vsa ON p.id = vsa.passport_id
                WHERE cp.email != ''
            ";

        public async Task<List<Recipient>> GetCustomersWithBirthdayToday()
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            string sql = $@"
                  {_commonSQL}
                  AND p.is_primary = 1 
                  AND DATE_FORMAT(p.date_of_birth, '%m-%d') = DATE_FORMAT(CURDATE(), '%m-%d')";

            var result = await connection.QueryAsync<Recipient>(sql);
            return [.. result];
        }

        public async Task<List<Recipient>> GetCustomersWithPassportExpiryFromSixMonths(string date)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            string sql = $@"
                  {_commonSQL}
                  AND p.ExpiryDate = '{date}'";

            var result = await connection.QueryAsync<Recipient>(sql);
            return [.. result];

        }

        public async Task<List<Recipient>> GetCustomersWithVisaExpiryFromThreeMonths(string date)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            string sql = $@"
                  {_commonSQL}
                  AND vsa.expire_date = '{date}'";

            var result = await connection.QueryAsync<Recipient>(sql);
            return [.. result];
        }
    }
}
