namespace Warehouse.Contracts;

public class IAllocateInventory
{
    Guid AllocationId { get; }

    string ItemNumber { get; }

    decimal Quantity { get; }
}