using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AIServices.Service;
using AIServices.Service.Interface;
using System;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

// Add HttpClient for API calls
builder.Services.AddHttpClient();

// Register OpenAI service
builder.Services.AddSingleton<OpenAIService>();
builder.Services.AddSingleton<IOpenAIService>(sp => sp.GetRequiredService<OpenAIService>());

// Register the AI Search service for ProjectDocument
builder.Services.AddSingleton<IAISearchService<AIServices.Model.ProjectDocument>, AISearchService<AIServices.Model.ProjectDocument>>();

// Register the RagContextService for both ProjectDocument and ChatRequest
builder.Services.AddSingleton<IRagContextService<AIServices.Model.ProjectDocument>, RagContextService<AIServices.Model.ProjectDocument>>();
builder.Services.AddSingleton<IRagContextService<ChatBotApi.Model.ChatRequest>, RagContextService<ChatBotApi.Model.ChatRequest>>();

// Register logging for services
builder.Services.AddLogging();

// Register the Semantic Kernel service for ProjectDocument
builder.Services.AddSingleton<ISemanticKernelService<AIServices.Model.ProjectDocument>>(sp =>
{
    var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger<SemanticKernelService<AIServices.Model.ProjectDocument>>();
    var config = sp.GetRequiredService<IConfiguration>();
    var aiSearchService = sp.GetRequiredService<IAISearchService<AIServices.Model.ProjectDocument>>();
    var openAIService = sp.GetRequiredService<IOpenAIService>();
    var ragContextService = sp.GetRequiredService<IRagContextService<AIServices.Model.ProjectDocument>>();
    return new SemanticKernelService<AIServices.Model.ProjectDocument>(config, logger, aiSearchService, openAIService, ragContextService);
});

// Register the Semantic Kernel service for ChatRequest
builder.Services.AddSingleton<ISemanticKernelService<ChatBotApi.Model.ChatRequest>>(sp =>
{
    var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger<SemanticKernelService<ChatBotApi.Model.ChatRequest>>();
    var config = sp.GetRequiredService<IConfiguration>();
    var openAIService = sp.GetRequiredService<IOpenAIService>();
    var ragContextService = sp.GetRequiredService<IRagContextService<ChatBotApi.Model.ChatRequest>>();
    // Pass null for aiSearchService since it's not needed for regular chat
    return new SemanticKernelService<ChatBotApi.Model.ChatRequest>(config, logger, null, openAIService, ragContextService);
});

var app = builder.Build();

app.Run();