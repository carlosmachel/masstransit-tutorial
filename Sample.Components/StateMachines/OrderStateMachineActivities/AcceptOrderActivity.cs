using MassTransit;
using Sample.Contracts;

namespace Sample.Components.StateMachines.OrderStateMachineActivities
{
    public class AcceptOrderActivity : IStateMachineActivity<OrderState, IOrderAccepted>
    {
        public void Probe(ProbeContext context)
        {
           context.CreateScope("accept-order");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<OrderState, IOrderAccepted> context, IBehavior<OrderState, IOrderAccepted> next)
        {
            Console.WriteLine("Order is {0}", context.Message.OrderId);

            var consumeContext = context.GetPayload<ConsumeContext>();

            var sendEndpoint = await consumeContext.GetSendEndpoint(new Uri("exchange:fufill-order"));

            await sendEndpoint.Send<IFufillOrder>(new
            {
                context.Message.OrderId
            });

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<OrderState, IOrderAccepted, TException> context, IBehavior<OrderState, IOrderAccepted> next) where TException : Exception
        {
            return next.Faulted(context);
        }

       
    }
}
