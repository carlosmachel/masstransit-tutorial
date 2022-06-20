namespace Sample.Contracts;

public interface IOrderSubmited
{
    Guid OrderId { get; }
    DateTime Timestamp { get; }
    string CustomerNumber { get; }
}
