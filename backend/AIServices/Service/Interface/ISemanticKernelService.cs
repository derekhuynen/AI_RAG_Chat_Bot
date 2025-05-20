using System.Collections.Generic;
using System.Threading.Tasks;

namespace AIServices.Service.Interface
{
    /// <summary>
    /// Interface for Semantic Kernel chat completion services
    /// </summary>
    /// <typeparam name="T">The document type for retrieval in RAG scenarios</typeparam>
    public interface ISemanticKernelService<T> where T : class
    {
        /// <summary>
        /// Gets a chat completion for the prompt
        /// </summary>
        /// <param name="prompt">The user prompt</param>
        /// <param name="modelName">The model to use</param>
        /// <returns>The generated text</returns>
        Task<string> GetChatCompletionAsync(string prompt, string modelName);

        /// <summary>
        /// Gets a chat completion for the prompt with optional context
        /// </summary>
        /// <param name="prompt">The user prompt</param>
        /// <param name="modelName">The model to use</param>
        /// <param name="context">Optional context to include with the prompt</param>
        /// <returns>The generated text</returns>
        Task<string> GetChatCompletionAsync(string prompt, string modelName, string? context = null);

        /// <summary>
        /// Gets a RAG chat completion for the prompt
        /// </summary>
        /// <param name="prompt">The user prompt</param>
        /// <param name="modelName">The model to use</param>
        /// <param name="top">Maximum number of documents to retrieve</param>
        /// <returns>The generated text</returns>
        Task<string> GetRagChatCompletionAsync(string prompt, string modelName, int top = 3);

        /// <summary>
        /// Gets a RAG chat completion for the prompt with citations
        /// </summary>
        /// <param name="prompt">The user prompt</param>
        /// <param name="modelName">The model to use</param>
        /// <param name="top">Maximum number of documents to retrieve</param>
        /// <returns>Tuple containing the answer and citations</returns>
        Task<(string Answer, IList<T> Citations)> GetRagChatCompletionWithCitationsAsync(string prompt, string modelName, int top = 3);
    }
}
