using MassTransit;
using Sample.Contracts;

namespace Sample.Components.Consumers
{
    public class FufillOrderConsumer : IConsumer<IFufillOrder>
    {
        public async Task Consume(ConsumeContext<IFufillOrder> context)
        {
            var builder = new RoutingSlipBuilder(NewId.NextGuid());
            //acctivity arguments
            //variables

            builder.AddActivity("AllocateInventory", new Uri("queue:allocate-inventory_execute"), new
            {
                ItemNumber = "ITEM123",
                Quantity = 10.0m
            });

            builder.AddVariable("OrderId", context.Message.OrderId);

            var routingSlip = builder.Build();

            await context.Execute(routingSlip);
        }
    }
}
