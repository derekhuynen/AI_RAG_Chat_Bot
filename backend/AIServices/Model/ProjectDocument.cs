using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AIServices.Model
{
    /// <summary>
    /// Model for project documents indexed/searched in Azure AI Search
    /// </summary>
    public class ProjectDocument
    {
        /// <summary>
        /// Unique identifier for the project
        /// </summary>
        [JsonPropertyName("id")]
        public string? id { get; set; }

        /// <summary>
        /// Project title
        /// </summary>
        [JsonPropertyName("title")]
        public string? title { get; set; }

        /// <summary>
        /// Project description
        /// </summary>
        [JsonPropertyName("description")]
        public string? description { get; set; }

        /// <summary>
        /// List of technologies used in the project
        /// </summary>
        [JsonPropertyName("tech_stack")]
        public List<string>? tech_stack { get; set; }

        /// <summary>
        /// Date range for the project (e.g., "2020-2021")
        /// </summary>
        [JsonPropertyName("date_range")]
        public string? date_range { get; set; }

        /// <summary>
        /// Additional metadata about the project
        /// </summary>
        [JsonPropertyName("metadata")]
        public string? metadata { get; set; }

        /// <summary>
        /// Full text content of the project
        /// </summary>
        [JsonPropertyName("raw_text")]
        public string? raw_text { get; set; }

        /// <summary>
        /// Vector embedding of the content
        /// </summary>
        [JsonPropertyName("content_vector")]
        public List<float>? content_vector { get; set; }
    }
}
