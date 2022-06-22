using MassTransit;
using Sample.Components.Notificator.interfaces;
using Sample.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Components.Consumers
{
    public class OrderAcceptedNotificationConsumer : IConsumer<IOrderAcceptedNotification>
    {
        private readonly IOrderHubNotificator orderHubNotificator;
        public OrderAcceptedNotificationConsumer(IOrderHubNotificator orderHubNotificator)
        {
            this.orderHubNotificator = orderHubNotificator;
        }

        public async Task Consume(ConsumeContext<IOrderAcceptedNotification> context)
        {
            await orderHubNotificator.NotifyOrderAcceptedAsync<IOrderAcceptedNotification>(context.Message);           
        }
    }
}
