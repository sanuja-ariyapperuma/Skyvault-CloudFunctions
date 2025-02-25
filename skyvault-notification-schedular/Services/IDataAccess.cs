using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skyvault_notification_schedular.Services
{
    public interface IDataAccess
    {
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null);
    }
}
