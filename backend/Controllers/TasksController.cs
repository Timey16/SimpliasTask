using backend.Entities;
using backend.Hubs;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace backend.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private ITaskService _taskService;
        private readonly IHubContext<NotificationsHub> _notificationsHub;

        public TasksController(ITaskService taskService, IHubContext<NotificationsHub> notificationsHub)
        {
            _taskService = taskService;
            _notificationsHub = notificationsHub;
        }

        [HttpPost]
        public async Task<ActionResult<TaskEntity>> CreateTask(TaskEntity task)
        {
            if (task != null)
            {
                var newTask = await _taskService.CreateTaskAsync(task);
                await _notificationsHub.Clients.All.SendAsync("TaskCreated", task);
                return Ok(newTask);
            }
            return BadRequest("createTaskNull");
        }

        [HttpGet]
        public async Task<ActionResult<TaskEntity[]>> GetTasks()
        {
            var tasks = await _taskService.GetTasksAsync();
            return Ok(tasks);
        }

        [HttpPut("{id}/complete")]
        public async Task<ActionResult<TaskEntity>> CompleteTask(int id)
        {
            var task = await _taskService.CompleteTaskAsync(id);
            await _notificationsHub.Clients.All.SendAsync("TaskCompleted", id);
            return Ok(task);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTask(int id)
        {
            await _taskService.DeleteTaskAsync(id);
            await _notificationsHub.Clients.All.SendAsync("TaskDeleted", id);
            return Ok();
        }
    }
}