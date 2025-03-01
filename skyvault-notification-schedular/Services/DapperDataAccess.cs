using Dapper;
using MySql.Data.MySqlClient;

namespace skyvault_notification_schedular.Services
{
    public class DapperDataAccess(string connectionString) : IDataAccess
    {

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            var result = await connection.QueryAsync<T>(sql, param);
            return result;
        }
    }
}
