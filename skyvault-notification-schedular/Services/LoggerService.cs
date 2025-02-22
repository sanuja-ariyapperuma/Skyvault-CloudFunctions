using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
