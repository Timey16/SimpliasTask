using backend.Entities;
using DeepSeek.Core;
using DeepSeek.Core.Models;
using Newtonsoft.Json;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace backend.Services
{
    public class AIService : IAIService
    {
        private int _maxRetries = 3;
        public string? ErrorMsg { get; private set; }
        public JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            TypeInfoResolver = SourceGenerationContext.Default,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };
        protected HttpClient http;

        public AIService() {
            http = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:11434/api/"),
                Timeout = TimeSpan.FromSeconds(300),
            };
        }

        public async Task<Priority> getPriorityFromDescriptionAsync(string title, string description)
        {
            var completionText = await sendQueryAsync(
                "Create a suggested priority for the following title and description for a given task. Answer in only a single word.\n" +
                "Allowed answers are only \"low\", \"medium\" and \"high\" for the priority of said task.\n" +
                "The title of the tasks is: \"" + title + "\"\n"+
                "The description of the task is: \"" + description + "\"");

            var thinkingStart = completionText.IndexOf("<think>");
            var thinkingEnd = completionText.IndexOf("</think>") + "</think>".Length;
            var thinkingMessage = completionText.Substring(thinkingStart, thinkingEnd - thinkingStart);

            var normalizedText = completionText.Substring(thinkingMessage.Length).Trim().ToLower();
            switch (normalizedText)
            {
                case "low": return Priority.LOW;
                case "medium": return Priority.MEDIUM;
                case "high": return Priority.HIGH;
                default: return Priority.UNKNOWN;
            }
        }

        private async Task<string> sendQueryAsync(string query, int retries = 0)
        {
            var request = new ChatRequest
            {
                Messages = [
                    Message.NewSystemMessage("Your task is to figure out the priority of a given task."),
                    Message.NewUserMessage(query)
                ],
                // Specify the model
                Model = "deepseek-r1:1.5b",
                Stream = false,
            };

            var result = await ChatAsync(request, new CancellationToken());

            if (result is null && _maxRetries > retries)
            {
                retries++;
                return await sendQueryAsync(query, retries);
            }
            else if(result is null)
            {
                throw new Exception(ErrorMsg);
            }

            return result?.Message?.Content ?? string.Empty;
        }



        public async Task<ChatResponse?> ChatAsync(ChatRequest request, CancellationToken cancellationToken)
        {
            request.Stream = false;
            StringContent content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await http.PostAsync("chat", content, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                string text = await response.Content.ReadAsStringAsync();
                ErrorMsg = response.StatusCode.ToString() + text;
                return null;
            }

            string text2 = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(text2))
            {
                ErrorMsg = "empty response";
                return null;
            }

            return JsonConvert.DeserializeObject<ChatResponse>(text2);
        }
    }

    public class ChatResponse
    {
        public ResponseMessage Message { get; set; }
        public string Model { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("done_reason")]
        public string DoneReason { get; set; }
        public bool Done { get; set; }
        [JsonPropertyName("total_duration")]
        public long TotalDuration { get; set; }
        [JsonPropertyName("load_duration")]
        public long LoadDuration { get; set; }
        [JsonPropertyName("prompt_eval_count")]
        public int PromptEvalCount { get; set; }
        [JsonPropertyName("prompt_eval_duration")]
        public long PromptEvalDuration { get; set; }
        [JsonPropertyName("eval_count")]
        public int EvalCount { get; set; }
        [JsonPropertyName("eval_duration")]
        public long EvalDuration { get; set; }
    }

    public class ResponseMessage
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }
}
