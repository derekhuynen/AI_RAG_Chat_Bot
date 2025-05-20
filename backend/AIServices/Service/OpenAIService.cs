using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using AIServices.Service.Interface;
using System.Net.Http.Headers;
using System.Collections.Generic;

namespace AIServices.Service
{
    public class OpenAIService : IOpenAIService
    {
        private readonly string _endpoint;
        private readonly string _apiKey;
        private readonly string _deployment;
        private readonly string _apiVersion;
        private readonly HttpClient _httpClient;

        public OpenAIService(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _endpoint = config["OPENAI_ENDPOINT"]?.TrimEnd('/') ?? throw new InvalidOperationException("OPENAI_ENDPOINT not set");
            _apiKey = config["OPENAI_API_KEY"] ?? throw new InvalidOperationException("OPENAI_API_KEY not set");
            _deployment = config["OPENAI_EMBEDDING_DEPLOYMENT"] ?? "text-embedding-ada-002";
            _apiVersion = config["OPENAI_API_VERSION"] ?? "2024-04-01-preview";
            _httpClient = httpClientFactory.CreateClient();
        }

        // Constructor for direct initialization (used by UploadProjects)
        public OpenAIService(string endpoint, string apiKey, string deployment)
        {
            _endpoint = endpoint.TrimEnd('/');
            _apiKey = apiKey;
            _deployment = deployment;
            _apiVersion = Environment.GetEnvironmentVariable("OPENAI_API_VERSION") ?? "2024-04-01-preview";
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        // Constructor that loads settings from environment variables
        public OpenAIService()
        {
            _endpoint = Environment.GetEnvironmentVariable("OPENAI_ENDPOINT")?.TrimEnd('/') ?? throw new InvalidOperationException("OPENAI_ENDPOINT not set");
            _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? throw new InvalidOperationException("OPENAI_API_KEY not set");
            _deployment = Environment.GetEnvironmentVariable("OPENAI_EMBEDDING_DEPLOYMENT") ?? "text-embedding-ada-002";
            _apiVersion = Environment.GetEnvironmentVariable("OPENAI_API_VERSION") ?? "2024-04-01-preview";
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        public async Task<float[]> GetEmbeddingAsync(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                throw new ArgumentException("Input text for embedding cannot be null or empty.", nameof(prompt));

            var url = $"{_endpoint}/openai/deployments/{_deployment}/embeddings?api-version={_apiVersion}";
            var payload = new { input = prompt, model = _deployment };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            // Use the authorization header if it's already set, otherwise add api-key header
            if (_httpClient.DefaultRequestHeaders.Authorization == null)
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("api-key", _apiKey);
            }

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Failed to get embedding from OpenAI: {ex.Message}", ex);
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            if (!root.TryGetProperty("data", out var data) || data.GetArrayLength() == 0)
                throw new Exception("No embedding data returned from OpenAI.");
            var embedding = data[0].GetProperty("embedding");
            var vector = new List<float>();
            foreach (var v in embedding.EnumerateArray())
                vector.Add(v.GetSingle());
            return vector.ToArray();
        }

        // Method that returns List<float> to maintain compatibility with UploadProjects
        public async Task<List<float>> GetEmbeddingListAsync(string text)
        {
            var array = await GetEmbeddingAsync(text);
            return new List<float>(array);
        }
    }
}
