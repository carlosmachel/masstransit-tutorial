namespace Warehouse.Contracts;

public interface IInventoryAllocated
{
    Guid AllocatedId { get; }
    string ItemNumber { get; }   
    decimal Quantity { get; }
}