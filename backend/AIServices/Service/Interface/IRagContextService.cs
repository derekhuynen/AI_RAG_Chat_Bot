using System.Threading.Tasks;
using AIServices.Model;

namespace AIServices.Service.Interface
{
    /// <summary>
    /// Interface for RAG (Retrieval-Augmented Generation) context services
    /// </summary>
    /// <typeparam name="T">The document type for retrieval</typeparam>
    public interface IRagContextService<T> where T : class
    {
        /// <summary>
        /// Gets context and citations for a given prompt
        /// </summary>
        /// <param name="prompt">The user prompt</param>
        /// <param name="top">Maximum number of documents to retrieve</param>
        /// <returns>Context and citations</returns>
        Task<RagContextResult<T>> GetContextWithCitationsAsync(string prompt, int top);
    }
}
