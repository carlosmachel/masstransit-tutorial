using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Components.Notificator.interfaces
{
    public interface IOrderHubNotificator
    {        
        Task NotifyOrderAcceptedAsync<T>(T message);
    }
}
