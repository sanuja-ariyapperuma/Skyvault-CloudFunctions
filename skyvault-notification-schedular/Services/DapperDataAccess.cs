using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skyvault_notification_schedular.Services
{
    public class DapperDataAccess : IDataAccess
    {
        private readonly IDbConnection _connection;

        public DapperDataAccess(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null)
        {
            return await _connection.QueryAsync<T>(sql, param);
        }
    }
}
