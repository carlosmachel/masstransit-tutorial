namespace Sample.Contracts;

public interface IOrderAccepted
{
    Guid OrderId { get; }
    DateTime Timestamp { get; }
}