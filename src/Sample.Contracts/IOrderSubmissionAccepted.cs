namespace Sample.Contracts;

public interface IOrderSubmissionAccepted
{
    Guid OrderId { get; }

    DateTime TimeStamp { get; }

    string CustomerNumber { get; }
}
