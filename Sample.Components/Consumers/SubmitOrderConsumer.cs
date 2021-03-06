using MassTransit;
using Microsoft.Extensions.Logging;
using Sample.Contracts;

namespace Sample.Components.Consumers;

public class SubmitOrderConsumer : IConsumer<ISubmitOrder>
{
    private readonly ILogger<SubmitOrderConsumer> _logger;

    public SubmitOrderConsumer(ILogger<SubmitOrderConsumer> logger)
    {
        _logger = logger;
    }

    public SubmitOrderConsumer()
    {
    }

    public async Task Consume(ConsumeContext<ISubmitOrder> context)
    {
        _logger?.Log(LogLevel.Debug, "SubmitOrderConsumer: {ConsumerNumber}", context.Message.CustomerNumber);

        if (context.Message.CustomerNumber.Contains("TEST"))
        {
            if (context.RequestId != null)
            {
                await context.RespondAsync<IOrderSubmissionRejected>(new
                {
                    TimeStamp = InVar.Timestamp,
                    context.Message.OrderId,
                    context.Message.CustomerNumber,
                    Reason = $"Test Customer cannot submit orders: {context.Message.CustomerNumber}"
                });

            }


            return;
        }

        await context.Publish<IOrderSubmited>(new
        {
            context.Message.OrderId,
            context.Message.Timestamp,
            context.Message.CustomerNumber
        });

        if (context.RequestId != null)
            await context.RespondAsync<IOrderSubmissionAccepted>(new
            {
                TimeStamp = InVar.Timestamp,
                context.Message.OrderId,
                context.Message.CustomerNumber
            });
    }
}

public class SubmitOrderConsumerDefinition : ConsumerDefinition<SubmitOrderConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<SubmitOrderConsumer> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(3, 1000));
    }
}