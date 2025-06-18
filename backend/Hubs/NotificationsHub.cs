using backend.Entities;
using Microsoft.AspNetCore.SignalR;

namespace backend.Hubs
{
    public class NotificationsHub : Hub<INotificationClient>
    {
        public override async Task OnConnectedAsync()
        {
            Clients.Client(Context.ConnectionId).ReceiveNotification($"Client has been connected to the SignalR hub. {Context.User.Identity.Name");
            await base.OnConnectedAsync();
        }
    }

    public interface INotificationClient
    {
        Task ReceiveNotification(string message);
        Task ReceiveNewPriority(int id, Priority priority);

    }
}
