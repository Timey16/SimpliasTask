using backend.Entities;
using Microsoft.AspNetCore.SignalR;

namespace backend.Hubs
{
    public class NotificationsHub : Hub<INotificationClient>
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.Client(Context.ConnectionId).ReceiveNotification($"Client {Context?.User?.Identity?.Name} has been connected to the SignalR hub.");
            await base.OnConnectedAsync();
        }
    }

    public interface INotificationClient
    {
        Task ReceiveNotification(string message);
        Task ReceiveNewPriority(int id, Priority priority);
    }
}
