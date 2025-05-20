using AIServices.Service;

namespace UploadProjects.Service
{
    /// <summary>
    /// Service responsible for embedding and uploading project documents to Azure AI Search
    /// </summary>
    public class ProjectUploadService
    {
        private readonly OpenAIService? _openAi;
        private readonly AISearchService<AIServices.Model.ProjectDocument>? _aiSearch;

        /// <summary>
        /// Initializes a new instance of the ProjectUploadService
        /// </summary>
        public ProjectUploadService()
        {
            var openAiEndpoint = Environment.GetEnvironmentVariable("OPENAI_ENDPOINT");
            var openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            var openAiEmbedDeployment = Environment.GetEnvironmentVariable("OPENAI_EMBEDDING_DEPLOYMENT");

            var searchEndpoint = Environment.GetEnvironmentVariable("AZURE_SEARCH_ENDPOINT");
            var searchIndex = Environment.GetEnvironmentVariable("AZURE_SEARCH_INDEX");
            var searchApiKey = Environment.GetEnvironmentVariable("AZURE_SEARCH_API_KEY");

            // Validate OpenAI settings
            if (string.IsNullOrEmpty(openAiEndpoint) || string.IsNullOrEmpty(openAiApiKey) || string.IsNullOrEmpty(openAiEmbedDeployment))
            {
                Console.WriteLine("Missing OpenAI configuration. Please check your environment variables.");
                Console.WriteLine($"OPENAI_ENDPOINT: {(string.IsNullOrEmpty(openAiEndpoint) ? "Missing" : "Set")}");
                Console.WriteLine($"OPENAI_API_KEY: {(string.IsNullOrEmpty(openAiApiKey) ? "Missing" : "Set")}");
                Console.WriteLine($"OPENAI_EMBEDDING_DEPLOYMENT: {(string.IsNullOrEmpty(openAiEmbedDeployment) ? "Missing" : "Set")}");
                _openAi = null;
            }
            else
            {
                _openAi = new OpenAIService(openAiEndpoint, openAiApiKey, openAiEmbedDeployment);
            }

            // Validate AI Search settings
            if (string.IsNullOrEmpty(searchEndpoint) || string.IsNullOrEmpty(searchIndex) || string.IsNullOrEmpty(searchApiKey))
            {
                Console.WriteLine("Missing Azure AI Search configuration. Please check your environment variables.");
                Console.WriteLine($"AZURE_SEARCH_ENDPOINT: {(string.IsNullOrEmpty(searchEndpoint) ? "Missing" : "Set")}");
                Console.WriteLine($"AZURE_SEARCH_INDEX: {(string.IsNullOrEmpty(searchIndex) ? "Missing" : "Set")}");
                Console.WriteLine($"AZURE_SEARCH_API_KEY: {(string.IsNullOrEmpty(searchApiKey) ? "Missing" : "Set")}");
                _aiSearch = null;
            }
            else
            {
                _aiSearch = new AISearchService<AIServices.Model.ProjectDocument>(searchEndpoint, searchIndex, searchApiKey);
            }
        }

        /// <summary>
        /// Embeds and uploads a list of project documents to Azure AI Search
        /// </summary>
        /// <param name="projects">List of projects to embed and upload</param>
        /// <returns>A tuple with the count of successful and failed documents</returns>
        public async Task<(int success, int fail)> EmbedAndUploadAsync(List<Model.ProjectDocument> projects)
        {
            if (_openAi == null)
            {
                Console.WriteLine("OpenAIService is not initialized.");
                return (0, projects.Count);
            }

            if (_aiSearch == null)
            {
                Console.WriteLine("AISearchService is not initialized.");
                return (0, projects.Count);
            }

            int total = projects.Count;
            int success = 0, fail = 0;
            for (int i = 0; i < total; i++)
            {
                var project = projects[i];
                try
                {
                    Console.WriteLine($"Processing project {i + 1}/{total}: {project.id} - {project.title}");
                    float[] embeddings = await _openAi.GetEmbeddingAsync(project.raw_text ?? "");
                    project.content_vector = new List<float>(embeddings);
                    success++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error embedding project {project.id}: {ex.Message}");
                    fail++;
                }
            }
            try
            {
                // Convert to AIServices.Model.ProjectDocument
                var aiServicesDocuments = projects.Select(doc => new AIServices.Model.ProjectDocument
                {
                    id = doc.id,
                    title = doc.title,
                    description = doc.description,
                    tech_stack = doc.tech_stack,
                    date_range = doc.date_range,
                    metadata = doc.metadata,
                    raw_text = doc.raw_text,
                    content_vector = doc.content_vector
                }).ToList();

                // Call AIServices directly
                if (_aiSearch != null)
                {
                    Console.WriteLine($"Uploading {success} documents to Azure AI Search...");
                    await _aiSearch.UploadDocumentsAsync(aiServicesDocuments);
                    Console.WriteLine($"Upload complete. Success: {success}, Failed: {fail}");
                }
                else
                {
                    Console.WriteLine("AISearchService is not initialized.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading to Azure AI Search: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
            return (success, fail);
        }
    }
}