using MassTransit;
using Warehouse.Contracts;

namespace Warehouse.Components;

public class AllocateInventoryConsumer : IConsumer<IAllocateInventory>
{
    public async Task Consume(ConsumeContext<IAllocateInventory> context)
    {
        throw new NotImplementedException();
    }
}