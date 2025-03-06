using Dapper;
using MySql.Data.MySqlClient;

namespace skyvault_notification_schedular.Services
{
    public class DapperDataAccess : IDataAccess
    {
        private readonly string _connectionString;

        public DapperDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> ExecuteAsync(string sql, object param = null)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            return await connection.ExecuteAsync(sql, param);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            var result = await connection.QueryAsync<T>(sql, param);
            return result;
        }
    }
}
