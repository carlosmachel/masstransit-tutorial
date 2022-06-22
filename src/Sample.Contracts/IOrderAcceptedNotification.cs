namespace Sample.Contracts;

public interface IOrderAcceptedNotification
{
    Guid OrderId { get; }
}
