using backend.Entities;
using backend.Entities.Contexts;
using backend.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Threading.Tasks;

namespace backend.Services
{
    public class TaskService : ITaskService
    {
        private readonly IAIService _aiService;
        private readonly IHubContext<NotificationsHub> _notificationsHub;
        private readonly TaskEntityDbContext _context;

        public TaskService(IAIService aiService, IHubContext<NotificationsHub> notificationsHub, TaskEntityDbContext context)
        {
            _aiService = aiService;
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

            _ = ReceiveNewPriorityAsync(newTask.TaskId, newTask.Name, newTask.Description);
            return newTask;
        }

        public async Task<TaskEntity[]> GetTasksAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await _context.Database.OpenConnectionAsync();
            var tasks = _context.Tasks.ToArray();
            foreach (var task in tasks)
            {
                if(task.Priority == Priority.UNSET)
                {
                    _ = ReceiveNewPriorityAsync(task.TaskId, task.Name, task.Description);
                }
            }
            return tasks;
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

        private async Task ReceiveNewPriorityAsync(int id, string title, string description)
        {

            var task = _context.Find<TaskEntity>(id);
            var priority = await _aiService.getPriorityFromDescriptionAsync(title, description);
            if (task == null)
            {
                return;
            }
            task.Priority = priority;

            //db has been dispose use anew.
            var optionsBuilder = new DbContextOptionsBuilder<TaskEntityDbContext>();
            optionsBuilder.UseSqlite("data source=database.db");
            using (var db = new TaskEntityDbContext(optionsBuilder.Options))
            {
                db.Update(task);
                await db.SaveChangesAsync();
                await _notificationsHub.Clients.All.SendAsync("TaskPriority", id, priority);
            }
        }
    }
}
