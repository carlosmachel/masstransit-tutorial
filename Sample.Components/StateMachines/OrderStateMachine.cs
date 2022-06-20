using MassTransit;
using Sample.Contracts;

namespace Sample.Components.StateMachines;

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public OrderStateMachine()
    {
        //Correlaciona o Correlation do estado com a OrderId como a correlacao.
        Event(() => OrderSubmitted, x => x.CorrelateById(a => a.Message.OrderId));
        Event(() => OrderStatusRequested, x =>
        {
            x.CorrelateById(a => a.Message.OrderId);
            x.OnMissingInstance(m => m.ExecuteAsync(async context =>
            {
                if (context.RequestId.HasValue)
                {
                    await context.RespondAsync<IOrderNotFound>(new { context.Message.OrderId });
                }
            }));
        });

        InstanceState(x => x.CurrentState);

        Initially(
            When(OrderSubmitted)
            .Then(context =>
            {
                context.Saga.SubmitDate = context.Message.Timestamp;
                context.Saga.CustomerNumber = context.Message.CustomerNumber;
                context.Saga.Updated = DateTime.UtcNow;
            })
            .TransitionTo(Submitted));

        //Indempotent
        During(Submitted,
            Ignore(OrderSubmitted));

        DuringAny(
            When(OrderStatusRequested)
            .RespondAsync(x => x.Init<IOrderStatus>(new {
                OrderId = x.Saga.CorrelationId, 
                State = x.Saga.CurrentState }
            )));
    }

    public State Submitted { get; private set; }
    public Event<IOrderSubmited> OrderSubmitted { get; private set; }
    public Event<ICheckOrder> OrderStatusRequested { get; private set; }
}
