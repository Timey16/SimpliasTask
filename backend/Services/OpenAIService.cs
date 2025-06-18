using OpenAI.Chat;
using backend.Entities;

namespace backend.Services
{
    public class OpenAIService : IOpenAIService
    {
        private ChatClient Client = new(model: "gpt-4o", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        public OpenAIService() { }

        public async Task<Priority> getPriorityFromDescriptionAsync(string description)
        {
            string completionText = await sendQueryAsync(
                "Create a suggested priority for the following description for a given task. Answer in only a single word.\n" +
                "Allowed answers are only \"low\", \"medium\" and \"high\" for the priority of said task. The description of the task is:\n" +
                description);

            string normalizedText = completionText.ToLower();
            switch (normalizedText)
            {
                case "low": return Priority.LOW;
                case "medium": return Priority.MEDIUM;
                case "high": return Priority.HIGH;
                default: return Priority.UNKNOWN;
            }
        }

        private async Task<string> sendQueryAsync(string query)
        {
            ChatCompletion completion = await Client.CompleteChatAsync(query);
            int retries = 0;
            if (completion.FinishReason == ChatFinishReason.Stop)
            {
                if (completion.Content is { Count: > 0 })
                {
                    var completeMessage = "";
                    foreach (var content in completion.Content)
                    {
                        completeMessage += content;
                    }
                    return completeMessage;
                }
            }
            else if(completion.FinishReason == ChatFinishReason.Length)
            {
                throw new Exception("The requested query was too long and would have required too many token");
            }
            else if (completion.FinishReason == ChatFinishReason.ContentFilter)
            {
                throw new Exception("The request was not fulfilled due to Content Filter rules");
            }

            return String.Empty;
        }
    }
}
