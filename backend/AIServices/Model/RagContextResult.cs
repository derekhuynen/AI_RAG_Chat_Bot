using System.Collections.Generic;

namespace AIServices.Model
{
    /// <summary>
    /// Represents the result of RAG (Retrieval-Augmented Generation) context retrieval,
    /// including the combined context text and the citation sources.
    /// </summary>
    /// <typeparam name="T">The document type for citations</typeparam>
    public class RagContextResult<T> where T : class
    {
        /// <summary>
        /// Gets or sets the combined context text from all retrieved documents.
        /// </summary>
        public string Context { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of documents that were retrieved as citations/sources for the context.
        /// </summary>
        public IList<T> Citations { get; set; } = new List<T>();
    }

    /// <summary>
    /// Non-generic RagContextResult using ProjectDocument - provided for backward compatibility
    /// </summary>
    public class RagContextResult : RagContextResult<ProjectDocument> { }
}
