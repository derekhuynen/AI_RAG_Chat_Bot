// Interface for AI Search Service

namespace AIServices.Service.Interface
{
    /// <summary>
    /// Interface for Azure AI Search operations
    /// </summary>
    /// <typeparam name="T">The document type</typeparam>
    public interface IAISearchService<T> where T : class
    {
        /// <summary>
        /// Uploads documents to Azure AI Search
        /// </summary>
        /// <param name="documents">The documents to upload</param>
        Task UploadDocumentsAsync(IEnumerable<T> documents);

        /// <summary>
        /// Performs a hybrid (text + vector) search with the provided query and embedding
        /// </summary>
        /// <param name="query">The text query</param>
        /// <param name="embedding">The vector embedding representation of the query</param>
        /// <param name="top">Maximum number of results to return</param>
        /// <returns>List of matching documents</returns>
        Task<IList<T>> HybridSearchAsync(string query, float[] embedding, int top = 3);
    }
}
