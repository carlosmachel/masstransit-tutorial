using MassTransit;
using Microsoft.Extensions.Logging;
using Sample.Contracts;

namespace Sample.Components;

public class SubmitOrderConsumer : IConsumer<SubmitOrder>
{
    private readonly ILogger<SubmitOrderConsumer> _logger;

    public SubmitOrderConsumer(ILogger<SubmitOrderConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SubmitOrder> context)
    {
        _logger.Log(LogLevel.Debug, "SubmitOrderConsumer: {ConsumerNumber}", context.Message.CustomerNumber);

        if (context.Message.CustomerNumber.Contains("TEST"))
        {
            await context.RespondAsync<OrderSubmissionRejected>(new
            {
                InVar.Timestamp,
                context.Message.OrderId,
                context.Message.CustomerNumber,
                Reason = $"Test Customer cannot submit orders: {context.Message.CustomerNumber}"
            });

            return;
        }

        await context.RespondAsync<OrderSubmissionAccepted>(new { 
            InVar.Timestamp, 
            context.Message.OrderId, 
            context.Message.CustomerNumber });
    }
}
