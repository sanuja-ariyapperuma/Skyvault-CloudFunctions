using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using skyvault_notification_schedular.Services;
using System.Data;
using System.Threading.Tasks;

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

        services.AddTransient<IDataAccess>(provider => new DapperDataAccess(_connectionString));
        services.AddSingleton<ICustomerRepository, CustomerRepository>();
        services.AddSingleton<IEmailService>(new BrevoEmailService());
        services.AddSingleton<ITemplateRepository>(provider => new TemplateRepository(_connectionString));
    })
    .Build();

await host.RunAsync();