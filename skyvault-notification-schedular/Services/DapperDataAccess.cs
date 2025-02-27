using Dapper;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

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
