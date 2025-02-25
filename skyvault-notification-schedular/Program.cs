using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using skyvault_notification_schedular.Services;
using System.Data;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        string? _connectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING");

        if (string.IsNullOrEmpty(_connectionString))
        {
            var logger = services.BuildServiceProvider().GetService<ILogger<Program>>();
            logger?.LogError("Connection string not found");
            Environment.Exit(1);
        }

        services.AddSingleton<IDbConnection>(sp => new SqlConnection(_connectionString));
        services.AddSingleton<ITemplateRepository>(new TemplateRepository(_connectionString));
        services.AddSingleton<IEmailService>(new BrevoEmailService());
    })
    .Build();

host.Run();
