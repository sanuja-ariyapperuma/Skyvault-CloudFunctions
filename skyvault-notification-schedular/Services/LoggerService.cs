using Microsoft.Extensions.Logging;

namespace skyvault_notification_schedular.Services
{
    public static class LoggerService
    {
        private static ILoggerFactory _loggerFactory;
        private static ILogger _logger;

        public static void Initialize(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger("GlobalLogger");
        }

        public static ILogger Log => _logger;
    }
}
