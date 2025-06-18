using backend.Entities;
using Microsoft.AspNetCore.Mvc;
using backend.Services;
using System.Threading.Tasks;

namespace FullStack.Server.Controllers
{
    [ApiController]
    [Route("tasks")]
    public class TaskController : ControllerBase
    {
        private ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost]
        public async Task<IResult> CreateTask(TaskEntity task)
        {
            if (task != null)
            {
                var newTask = await _taskService.CreateTask(task);
                return Results.Ok(newTask);
            }
            return Results.Empty;
        }

        [HttpGet]
        public async Task<IResult> GetTasks()
        {
            var tasks = await _taskService.GetTasks();
            return Results.Ok(tasks);
        }

        [HttpPut]
        [ActionName("complete")]
        public async Task<IResult> CompleteTask(int id)
        {
            var task = await _taskService.CompleteTask(id);
            return Results.Ok(task);
        }

        [HttpDelete]
        public async Task<IResult> DeleteTask(int id)
        {
            await _taskService.DeleteTask(id);
            return Results.Ok();
        }
    }
}
