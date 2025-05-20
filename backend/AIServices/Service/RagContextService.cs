using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using AIServices.Model;
using AIServices.Service.Interface;

namespace AIServices.Service
{
    /// <summary>
    /// Implementation of the RAG Context Service that combines OpenAI embeddings with Azure AI Search
    /// to provide context and citations for retrieval-augmented generation.
    /// </summary>
    /// <typeparam name="T">The document type</typeparam>
    public class RagContextService<T> : IRagContextService<T> where T : class
    {
        private readonly IOpenAIService _openAIService;
        private readonly IAISearchService<T> _aiSearchService;

        public RagContextService(OpenAIService openAIService, IAISearchService<T> aiSearchService)
        {
            _openAIService = openAIService ?? throw new ArgumentNullException(nameof(openAIService));
            _aiSearchService = aiSearchService ?? throw new ArgumentNullException(nameof(aiSearchService));
        }

        /// <summary>
        /// Gets context with citations based on a user prompt.
        /// </summary>
        /// <param name="prompt">The user prompt to retrieve context for</param>
        /// <param name="top">The maximum number of documents to retrieve</param>
        /// <returns>A RagContextResult containing the context and citations</returns>
        public async Task<RagContextResult<T>> GetContextWithCitationsAsync(string prompt, int top)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                throw new ArgumentException("Prompt cannot be empty.", nameof(prompt));

            float[] embedding = await _openAIService.GetEmbeddingAsync(prompt);
            var searchResults = await _aiSearchService.HybridSearchAsync(prompt, embedding, top);

            return new RagContextResult<T>
            {
                Context = string.Join("\n", searchResults.Select(d => GetTextFromDocument(d))),
                Citations = searchResults
            };
        }

        /// <summary>
        /// Helper method to extract text from a document. This should be customized based on type T.
        /// </summary>
        /// <param name="document">The document to extract text from</param>
        /// <returns>The text content of the document</returns>
        protected virtual string GetTextFromDocument(T document)
        {
            // Default implementation - override in derived classes for specific document types
            return document?.ToString() ?? string.Empty;
        }
    }
}
