using MassTransit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sample.Components.Consumers;
using Sample.Components.CourierActivities;
using Sample.Components.StateMachines;
using Sample.Components.StateMachines.OrderStateMachineActivities;
using Sample.Service;
using Serilog;
using Serilog.Events;

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
        services.AddScoped<AcceptOrderActivity>();
        services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
        services.AddMassTransit(x =>
        {
            x.AddConsumersFromNamespaceContaining<SubmitOrderConsumer>();
            x.AddActivitiesFromNamespaceContaining<AllocateInventoryActivity>();

            x.AddSagaStateMachine<OrderStateMachine, OrderState>(typeof(OrderStateMachineDefinition))
            .InMemoryRepository(); //no exemplo ele usa o redis.
           
            x.UsingRabbitMq((context, cfg) => {                
                cfg.ConfigureEndpoints(context); });
        });
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
 