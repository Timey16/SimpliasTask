using Microsoft.AspNetCore.SignalR;

namespace backend.Hubs
{
    public class NotificationsHub : Hub<INotificationClient>
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.Client(Context.ConnectionId).ReceiveNotificationAsync($"Client {Context?.User?.Identity?.Name} has been connected to the SignalR hub.");
            await base.OnConnectedAsync();
        }
    }

    public interface INotificationClient
    {
        Task ReceiveNotificationAsync(string message);
    }
}
