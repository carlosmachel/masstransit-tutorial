using Microsoft.AspNetCore.SignalR;

namespace Sample.Api.Hubs
{

    public class OrderNotificationHub : Hub
    {

        public void OrderAccepted(object message)
        {            
            Clients.All.SendAsync("OrderAccepted", message);
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine(Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            Console.WriteLine(Context.ConnectionId);
            await base.OnDisconnectedAsync(ex);
        }

    }
}
