using backend.Entities;
using backend.Entities.Contexts;
using backend.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace backend.Services
{
    public class TaskService : ITaskService
    {
        private readonly IOpenAIService _openAIService;
        private readonly IHubContext<NotificationsHub> _notificationsHub;
        private readonly TaskEntityDbContext _context;

        public TaskService(IOpenAIService openAIService, IHubContext<NotificationsHub> notificationsHub, TaskEntityDbContext context)
        {
            _openAIService = openAIService;
            _notificationsHub = notificationsHub;
            _context = context;
        }

        public async Task<TaskEntity> CreateTaskAsync(TaskEntity task)
        {
            var newTask = _context.Add(task).Entity;
            await _context.SaveChangesAsync();

            if (newTask == null)
            {
                throw new Exception($"Task of name {task.Name} couldn't be created");
            }

            _ = ReceiveNewPriorityAsync(newTask.TaskId, newTask.Description);
            return newTask;
        }

        public async Task<TaskEntity[]> GetTasksAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await _context.Database.OpenConnectionAsync();
            return _context.Tasks.ToArray();
        }

        public async Task<TaskEntity> CompleteTaskAsync(int id)
        {
            var task = _context.Find<TaskEntity>(id);
            if (task == null)
            {
                throw new Exception($"Task of id {id} couldn't be found");
            }
            task.Completed = true;

            _context.Update(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task DeleteTaskAsync(int id)
        {
            var task = _context.Find<TaskEntity>(id);
            if (task == null)
            {
                return;
            }

            _context.Remove(task);
            await _context.SaveChangesAsync();
        }

        private async Task ReceiveNewPriorityAsync(int id, string description)
        {
            var priority = await _openAIService.getPriorityFromDescriptionAsync(description);
            var task = _context.Find<TaskEntity>(id);
            if (task == null)
            {
                return;
            }
            task.Priority = priority;
            _context.Update(task);
            await _context.SaveChangesAsync();
            await _notificationsHub.Clients.All.SendAsync("TaskPriority", id, priority);
        }
    }
}
