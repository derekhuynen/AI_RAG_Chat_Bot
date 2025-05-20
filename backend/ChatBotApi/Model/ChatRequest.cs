using System.Text.Json.Serialization;

namespace ChatBotApi.Model
{
    /// <summary>
    /// Model for chat request payloads for both standard chat and RAG endpoints.
    /// </summary>
    public class ChatRequest
    {
        /// <summary>
        /// The user prompt to send to the chat model
        /// </summary>
        [JsonPropertyName("prompt")]
        public string? Prompt { get; set; }
    }
}
