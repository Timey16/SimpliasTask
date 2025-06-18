using backend.Entities;

namespace backend.Services
{
    public interface ITaskService
    {
        public Task<TaskEntity> CreateTask(TaskEntity task);
        public Task<TaskEntity[]> GetTasks();
        public Task<TaskEntity> CompleteTask(int id);
        public Task DeleteTask(int id);
    }
}
