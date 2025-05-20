using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using AIServices.Service.Interface;

namespace AIServices.Service
{
    /// <summary>
    /// Orchestrates chat and RAG (Retrieval-Augmented Generation) using Semantic Kernel, OpenAI, and Azure AI Search.
    /// </summary>
    /// <typeparam name="T">The document type for retrieval in RAG scenarios</typeparam>
    public class SemanticKernelService<T> : ISemanticKernelService<T> where T : class
    {
        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        private readonly Kernel _kernel;
        private readonly IAISearchService<T>? _aiSearchService;
        private readonly IOpenAIService _openAIService;
        private readonly KernelFunction _chatWithContextFunction;
        private readonly IRagContextService<T> _ragContextService;

        public SemanticKernelService(
            IConfiguration config,
            ILogger logger,
            IAISearchService<T>? aiSearchService,
            IOpenAIService openAIService,
            IRagContextService<T> ragContextService)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _aiSearchService = aiSearchService; // This can be null for ChatRequest scenarios
            _openAIService = openAIService ?? throw new ArgumentNullException(nameof(openAIService));
            _ragContextService = ragContextService ?? throw new ArgumentNullException(nameof(ragContextService));

            var builder = Kernel.CreateBuilder();

            // Get configuration values, first trying SemanticKernel section, then falling back to OpenAI values
            var endpoint = _config["SemanticKernel:Endpoint"];
            var apiKey = _config["SemanticKernel:ApiKey"];
            var chatDeployment = _config["SemanticKernel:ChatDeployment"];

            // Fall back to OpenAI values if SemanticKernel values are not found
            if (string.IsNullOrWhiteSpace(endpoint)) endpoint = _config["OPENAI_ENDPOINT"];
            if (string.IsNullOrWhiteSpace(apiKey)) apiKey = _config["OPENAI_API_KEY"];
            if (string.IsNullOrWhiteSpace(chatDeployment)) chatDeployment = _config["OPENAI_CHAT_DEPLOYMENT"] ?? "gpt-4.1";

            if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentException("Azure OpenAI configuration is missing. Please set SemanticKernel:Endpoint and SemanticKernel:ApiKey or OPENAI_ENDPOINT and OPENAI_API_KEY.");
            }

            builder.AddAzureOpenAIChatCompletion(
                deploymentName: chatDeployment,
                endpoint: endpoint,
                apiKey: apiKey
            );

            _kernel = builder.Build();

            // Register a semantic function for chat with context
            string chatPrompt = @"{{$context}}\nUser: {{$user_input}}\nAssistant:";
            _chatWithContextFunction = KernelFunctionFactory.CreateFromPrompt(chatPrompt, (Microsoft.SemanticKernel.PromptExecutionSettings?)null, "ChatWithContext");
        }

        /// <summary>
        /// Gets a chat completion for the prompt with optional context
        /// </summary>
        public async Task<string> GetChatCompletionAsync(string prompt, string modelName, string? context = null)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                _logger.LogWarning("Prompt cannot be empty.");
                throw new ArgumentException("Prompt cannot be empty.", nameof(prompt));
            }
            try
            {
                var arguments = new KernelArguments
                {
                    ["user_input"] = prompt,
                    ["context"] = context ?? string.Empty
                };
                var result = await _kernel.InvokeAsync(_chatWithContextFunction, arguments);
                return result.GetValue<string>() ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during chat completion.");
                throw;
            }
        }

        /// <summary>
        /// Gets a chat completion for the prompt (no context)
        /// </summary>
        public Task<string> GetChatCompletionAsync(string prompt, string modelName)
            => GetChatCompletionAsync(prompt, modelName, null);

        /// <summary>
        /// Gets a RAG chat completion for the prompt
        /// </summary>
        public async Task<string> GetRagChatCompletionAsync(string prompt, string modelName, int top = 3)
        {
            var (answer, _) = await GetRagChatCompletionWithCitationsAsync(prompt, modelName, top);
            return answer;
        }

        /// <summary>
        /// Gets a RAG chat completion for the prompt with citations
        /// </summary>
        public async Task<(string Answer, IList<T> Citations)> GetRagChatCompletionWithCitationsAsync(string prompt, string modelName, int top = 3)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                _logger.LogWarning("Prompt cannot be empty.");
                throw new ArgumentException("Prompt cannot be empty.", nameof(prompt));
            }
            try
            {
                var contextResult = await _ragContextService.GetContextWithCitationsAsync(prompt, top);
                if (string.IsNullOrWhiteSpace(contextResult.Context))
                {
                    _logger.LogWarning("AI Search returned empty context for prompt: {Prompt}", prompt);
                }
                else if (contextResult.Citations == null || !contextResult.Citations.Any())
                {
                    _logger.LogWarning("AI Search returned no results for prompt: {Prompt}", prompt);
                }
                else
                {
                    _logger.LogInformation("AI Search returned {Count} results for prompt: {Prompt}", contextResult.Citations.Count, prompt);
                }
                var arguments = new KernelArguments
                {
                    ["user_input"] = prompt,
                    ["context"] = contextResult.Context
                };
                var result = await _kernel.InvokeAsync(_chatWithContextFunction, arguments);
                return (result.GetValue<string>() ?? string.Empty, contextResult.Citations ?? new List<T>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during RAG chat completion. Prompt: {Prompt}", prompt);
                throw;
            }
        }
    }
}
