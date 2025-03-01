namespace skyvault_notification_schedular.Services
{
    public interface IDataAccess
    {
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null);
    }
}
