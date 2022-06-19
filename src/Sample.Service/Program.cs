using MassTransit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sample.Components;
using Sample.Service;
using Serilog;
using Serilog.Events;
using System.Reflection;

Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddJsonFile("appsettings.json", true);
        config.AddEnvironmentVariables();

        if (args != null)
            config.AddCommandLine(args);
    })
    .ConfigureLogging((hostingContext, logging) =>
    {
        logging.AddSerilog(dispose: true);
        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
        services.AddMassTransit(x =>
        {
            x.AddConsumer<SubmitOrderConsumer>();

            x.UsingRabbitMq((context, cfg) => { cfg.ConfigureEndpoints(context); });
        });
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
 