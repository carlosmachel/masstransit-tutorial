using Microsoft.AspNetCore.SignalR.Client;
using Sample.Components.Notificator.interfaces;

namespace Sample.Components.Notificator
{
    public class OrderHubNotificator : IOrderHubNotificator
    {

        private readonly HubConnection hubConnection;


        public OrderHubNotificator(string url)
        {
            this.hubConnection = new HubConnectionBuilder()
                                    .WithUrl(url)
                                    .WithAutomaticReconnect()
                                    .Build();

            
        }        

        public async Task NotifyOrderAcceptedAsync<T>(T message)
        {
            if (this.hubConnection.State == HubConnectionState.Disconnected)
                await this.hubConnection.StartAsync();

            await this.hubConnection.InvokeAsync("OrderAccepted", message);
        }
    }
}
