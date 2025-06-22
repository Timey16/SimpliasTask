using backend.Entities;

namespace backend.Services
{
    public interface IAIService
    {
        public Task<Priority> getPriorityFromDescriptionAsync(string title, string description);
    }
}
