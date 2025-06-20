using backend.Entities;
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

        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }

    public interface INotificationClient
    {
        Task ReceiveNotificationAsync(string message);
        Task ReceiveNewPriorityAsync(int id, Priority priority);
        Task ReceiveNewTaskAsync(TaskEntity task);
        Task ReceiveTaskDeletedAsync(int id);
        Task ReceiveTaskCompletedAsync(int id);
    }
}
