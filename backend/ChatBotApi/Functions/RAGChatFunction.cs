// Azure Function endpoint for Retrieval-Augmented Generation (RAG) chat with citations.
// Accepts a prompt, retrieves context from Azure AI Search, and returns a response with citations.
using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using ChatBotApi.Model;
using AIServices.Service.Interface;
using AIServices.Model;

namespace ChatBotApi.Functions
{
    /// <summary>
    /// Azure Function endpoint for Retrieval-Augmented Generation (RAG) chat with citations.
    /// Accepts a prompt, retrieves context from Azure AI Search, and returns a response with citations.
    /// </summary>
    public class RAGChatFunction
    {
        private readonly ILogger<RAGChatFunction> _logger;
        private readonly IConfiguration _config;
        private readonly ISemanticKernelService<ProjectDocument> _semanticKernel;

        public RAGChatFunction(
            IHttpClientFactory httpClientFactory,
            ILogger<RAGChatFunction> logger,
            IConfiguration config,
            ISemanticKernelService<ProjectDocument> semanticKernel)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _semanticKernel = semanticKernel ?? throw new ArgumentNullException(nameof(semanticKernel));
        }

        [Function("RAGChatFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("RAGChatFunction HTTP trigger processing a request.");

            // Read and deserialize the request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var chatRequest = JsonSerializer.Deserialize<ChatRequest>(requestBody);

            if (string.IsNullOrWhiteSpace(chatRequest?.Prompt))
            {
                _logger.LogWarning("Empty prompt received in RAGChatFunction");
                return new BadRequestObjectResult(new { error = "Prompt cannot be empty." });
            }

            try
            {
                var modelName = AvailableModel.Gpt41.GetModelName();
                _logger.LogInformation($"Requesting RAG chat completion with model: {modelName}");

                (string response, IList<ProjectDocument> citations) = await _semanticKernel.GetRagChatCompletionWithCitationsAsync(chatRequest.Prompt, modelName);

                _logger.LogInformation("RAGChatFunction completed successfully");
                return new OkObjectResult(new
                {
                    response,
                    citations = citations.Select(c => new
                    {
                        id = c.id,
                        title = c.title,
                        description = c.description,
                        tech_stack = c.tech_stack,
                        date_range = c.date_range,
                        metadata = c.metadata
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing RAG chat request");
                return new ObjectResult(new { error = "An error occurred processing your request." })
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
