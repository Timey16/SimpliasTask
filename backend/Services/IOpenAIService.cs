using backend.Entities;

namespace backend.Services
{
    public interface IOpenAIService
    {
        public Task<Priority> getPriorityFromDescriptionAsync(string description);
    }
}
