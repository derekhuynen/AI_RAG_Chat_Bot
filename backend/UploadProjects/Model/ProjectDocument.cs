using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace UploadProjects.Model
{
    /// <summary>
    /// Model for project documents to be uploaded to Azure AI Search
    /// </summary>
    public class ProjectDocument
    {
        /// <summary>
        /// Unique identifier for the project
        /// </summary>
        [JsonPropertyName("id")]
        public string id { get; set; } = string.Empty;

        /// <summary>
        /// Project title
        /// </summary>
        [JsonPropertyName("title")]
        public string title { get; set; } = string.Empty;

        /// <summary>
        /// Project description
        /// </summary>
        [JsonPropertyName("description")]
        public string description { get; set; } = string.Empty;

        /// <summary>
        /// List of technologies used in the project
        /// </summary>
        [JsonPropertyName("tech_stack")]
        public List<string> tech_stack { get; set; } = new List<string>();

        /// <summary>
        /// Date range for the project (e.g., "2020-2021")
        /// </summary>
        [JsonPropertyName("date_range")]
        public string date_range { get; set; } = string.Empty;

        /// <summary>
        /// Additional metadata about the project
        /// </summary>
        [JsonPropertyName("metadata")]
        public string metadata { get; set; } = string.Empty;

        /// <summary>
        /// Full text content of the project
        /// </summary>
        [JsonPropertyName("raw_text")]
        public string raw_text { get; set; } = string.Empty;

        /// <summary>
        /// Vector embedding of the content
        /// </summary>
        [JsonPropertyName("content_vector")]
        public List<float> content_vector { get; set; } = new List<float>();
    }
}