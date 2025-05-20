// Azure Function endpoint for standard chat using Azure OpenAI (no RAG).
// Accepts a prompt and returns a chat completion response.
using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using ChatBotApi.Model;
using AIServices.Service.Interface;

namespace ChatBotApi.Functions
{
    /// <summary>
    /// Azure Function endpoint for standard chat using Azure OpenAI (no RAG).
    /// Accepts a prompt and returns a chat completion response.
    /// </summary>
    public class ChatFunction
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ChatFunction> _logger;
        private readonly IConfiguration _config;
        private readonly ISemanticKernelService<ChatRequest> _semanticKernel;

        public ChatFunction(
            IHttpClientFactory httpClientFactory,
            ILogger<ChatFunction> logger,
            IConfiguration config,
            ISemanticKernelService<ChatRequest> semanticKernel)
        {
            _httpClient = httpClientFactory?.CreateClient() ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _semanticKernel = semanticKernel ?? throw new ArgumentNullException(nameof(semanticKernel));
        }

        [Function("ChatFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("ChatFunction HTTP trigger processing a request.");

            // Read and deserialize the request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var chatRequest = JsonSerializer.Deserialize<ChatRequest>(requestBody);

            if (string.IsNullOrWhiteSpace(chatRequest?.Prompt))
            {
                _logger.LogWarning("Empty prompt received in ChatFunction");
                return new BadRequestObjectResult(new { error = "Prompt cannot be empty." });
            }

            try
            {
                var modelName = AvailableModel.Gpt41.GetModelName();
                _logger.LogInformation($"Requesting chat completion with model: {modelName}");

                var response = await _semanticKernel.GetChatCompletionAsync(chatRequest.Prompt, modelName);

                _logger.LogInformation("ChatFunction completed successfully");
                return new OkObjectResult(new { response });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat request");
                return new ObjectResult(new { error = "An error occurred processing your request." })
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
