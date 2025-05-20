using System.Collections.Generic;
using System.Threading.Tasks;

namespace AIServices.Service.Interface
{
    /// <summary>
    /// Interface for OpenAI embedding operations
    /// </summary>
    public interface IOpenAIService
    {
        /// <summary>
        /// Gets text embedding as a float array
        /// </summary>
        /// <param name="text">The text to embed</param>
        /// <returns>Float array representing the embedding vector</returns>
        Task<float[]> GetEmbeddingAsync(string text);

        /// <summary>
        /// Gets text embedding as a list of floats
        /// (Used by UploadProjects for backward compatibility)
        /// </summary>
        /// <param name="text">The text to embed</param>
        /// <returns>List of floats representing the embedding vector</returns>
        Task<List<float>> GetEmbeddingListAsync(string text);
    }
}
