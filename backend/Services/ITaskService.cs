using backend.Entities;

namespace backend.Services
{
    public interface ITaskService
    {
        public Task<TaskEntity> CreateTaskAsync(TaskEntity task);
        public Task<TaskEntity[]> GetTasksAsync();
        public Task<TaskEntity> CompleteTaskAsync(int id);
        public Task DeleteTaskAsync(int id);
    }
}
