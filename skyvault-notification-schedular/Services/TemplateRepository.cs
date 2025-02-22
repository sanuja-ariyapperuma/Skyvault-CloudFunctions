using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skyvault_notification_schedular.Services
{
    public sealed class TemplateRepository(string connectionString) : ITemplateRepository
    {
        public async Task<string?> GetBirthdayImageURL()
        {
            const string query = "SELECT file FROM notification_templates WHERE Active = 1 AND notification_type = 1";

            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            return await connection.ExecuteScalarAsync<string>(query);
        }
    }
}
