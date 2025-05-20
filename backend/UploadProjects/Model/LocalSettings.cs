using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace UploadProjects.Model
{
    /// <summary>
    /// Model for deserializing settings from local.settings.json
    /// </summary>
    public class LocalSettings
    {
        /// <summary>
        /// Gets or sets the environment variable key-value pairs
        /// </summary>
        [JsonPropertyName("Values")]
        public Dictionary<string, string> Values { get; set; } = new Dictionary<string, string>();
    }
}
