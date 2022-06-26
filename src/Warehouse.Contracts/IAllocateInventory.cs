namespace Warehouse.Contracts;

public interface IAllocateInventory
{
    Guid AllocationId { get; }

    string ItemNumber { get; }

    decimal Quantity { get; }
}

public interface  AllocationReleaseRequested
{
    Guid AllocationId { get; }

    string Reason { get; }
}