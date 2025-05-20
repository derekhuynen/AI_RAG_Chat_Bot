using System.Text;
using System.Text.Json;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AIServices.Service.Interface;

namespace AIServices.Service
{
    public class AISearchService<T> : IAISearchService<T> where T : class
    {
        private readonly SearchClient _searchClient;
        private readonly ILogger? _logger;
        private readonly string _apiKey;

        public AISearchService(IConfiguration config, ILogger logger)
        {
            _logger = logger;

            var endpoint = config["AZURE_SEARCH_ENDPOINT"];
            var indexName = config["AZURE_SEARCH_INDEX"];
            var apiKey = config["AZURE_SEARCH_API_KEY"];

            if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(indexName) || string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("Azure AI Search configuration is missing. Please set AZURE_SEARCH_ENDPOINT, AZURE_SEARCH_INDEX, and AZURE_SEARCH_API_KEY.");

            _searchClient = new SearchClient(new Uri(endpoint), indexName, new AzureKeyCredential(apiKey));
            _apiKey = apiKey;
        }

        public AISearchService()
        {
            _logger = null;

            var endpoint = Environment.GetEnvironmentVariable("AZURE_SEARCH_ENDPOINT");
            var indexName = Environment.GetEnvironmentVariable("AZURE_SEARCH_INDEX");
            var apiKey = Environment.GetEnvironmentVariable("AZURE_SEARCH_API_KEY");

            if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(indexName) || string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("Azure AI Search configuration is missing. Please set AZURE_SEARCH_ENDPOINT, AZURE_SEARCH_INDEX, and AZURE_SEARCH_API_KEY.");

            _searchClient = new SearchClient(new Uri(endpoint), indexName, new AzureKeyCredential(apiKey));
            _apiKey = apiKey;
        }

        public AISearchService(string endpoint, string indexName, string apiKey)
        {
            _logger = null;
            if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(indexName) || string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("Azure AI Search configuration is missing. Please provide valid endpoint, indexName, and apiKey.");

            _searchClient = new SearchClient(new Uri(endpoint), indexName, new AzureKeyCredential(apiKey));
            _apiKey = apiKey;
        }

        public async Task UploadDocumentsAsync(IEnumerable<T> documents)
        {
            if (documents == null)
                throw new ArgumentNullException(nameof(documents));

            var batch = IndexDocumentsBatch.Create<T>();
            foreach (var document in documents)
            {
                if (document == null)
                    continue;
                batch.Actions.Add(IndexDocumentsAction.Upload(document));
            }
            try
            {
                await _searchClient.IndexDocumentsAsync(batch);
            }
            catch (RequestFailedException ex)
            {
                throw new Exception($"Azure AI Search indexing failed: {ex.Message}", ex);
            }
        }

        public async Task<IList<T>> HybridSearchAsync(string query, float[] embedding, int top = 3)
        {
            var results = new List<T>();
            try
            {
                var endpoint = _searchClient.Endpoint.ToString().TrimEnd('/');
                var indexName = _searchClient.IndexName;
                var url = $"{endpoint}/indexes/{indexName}/docs/search?api-version=2024-03-01-Preview";

                var requestBody = new
                {
                    search = query,
                    vectorQueries = new[]
                    {
                        new {
                            vector = embedding,
                            fields = "content_vector",
                            k = top,
                            kind = "vector"
                        }
                    },
                    top = top
                };

                var requestJson = JsonSerializer.Serialize(requestBody);

                if (_logger != null)
                {
                    _logger.LogDebug($"Azure AI Search HybridSearch Request URL: {url}");
                    _logger.LogDebug($"Azure AI Search HybridSearch Request Headers: api-key=***");
                    _logger.LogDebug($"Azure AI Search HybridSearch Request Body: {requestJson}");
                }

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("api-key", _apiKey);
                var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(url, content);

                var responseBody = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    if (_logger != null)
                    {
                        _logger.LogError($"Azure AI Search HybridSearch failed. Status: {response.StatusCode}, Body: {responseBody}");
                    }
                    throw new Exception($"Azure AI Search HybridSearch failed. Status: {response.StatusCode}, Body: {responseBody}");
                }

                using var doc = JsonDocument.Parse(responseBody);
                foreach (var docElem in doc.RootElement.GetProperty("value").EnumerateArray())
                {
                    var documentResult = JsonSerializer.Deserialize<T>(docElem.GetRawText());
                    if (documentResult != null)
                        results.Add(documentResult);
                }
                return results;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Error performing hybrid search on Azure AI Search");
                }
                throw;
            }
        }
    }
}
