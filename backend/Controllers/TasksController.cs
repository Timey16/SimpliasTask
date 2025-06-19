using backend.Entities;
using Microsoft.AspNetCore.Mvc;
using backend.Services;
using Microsoft.AspNetCore.Authorization;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<TaskEntity>> CreateTask(TaskEntity task)
        {
            if (task != null)
            {
                var newTask = await _taskService.CreateTask(task);
                return Ok(newTask);
            }
            return BadRequest("creatTaskNull");
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<TaskEntity[]>> GetTasks()
        {
            var tasks = await _taskService.GetTasks();
            return Ok(tasks);
        }

        [HttpPut("complete")]
        [Authorize]
        public async Task<ActionResult<TaskEntity>> CompleteTask(int id)
        {
            var task = await _taskService.CompleteTask(id);
            return Ok(task);
        }

        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> DeleteTask(int id)
        {
            await _taskService.DeleteTask(id);
            return Ok();
        }
    }
}
