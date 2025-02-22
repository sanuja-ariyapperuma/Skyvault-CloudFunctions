using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using skyvault_notification_schedular.Services;

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

        services.AddSingleton<ICustomerRepository>(new CustomerRepository(_connectionString));
        services.AddSingleton<ITemplateRepository>(new TemplateRepository(_connectionString));
        services.AddSingleton<IEmailService>(new BrevoEmailService());
    })
    .Build();

host.Run();
