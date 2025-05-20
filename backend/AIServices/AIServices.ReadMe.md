# AIServices Library

![GitHub](https://img.shields.io/github/license/yourusername/AI_RAG_Chat_Bot)

A comprehensive .NET library for building AI-powered applications with Retrieval-Augmented Generation (RAG) capabilities. This library provides services for embeddings generation, semantic search, and AI-powered chat completions, leveraging Azure OpenAI and Azure AI Search.

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Services](#services)
  - [OpenAI Service](#openai-service)
  - [AI Search Service](#ai-search-service)
  - [RAG Context Service](#rag-context-service)
  - [Semantic Kernel Service](#semantic-kernel-service)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Usage Examples](#usage-examples)
- [License](#license)

## 🔍 Overview

AIServices is a specialized .NET library designed to facilitate the development of AI-powered applications using Retrieval-Augmented Generation (RAG) techniques. It integrates seamlessly with Azure OpenAI and Azure AI Search to provide high-quality, context-aware AI responses based on your own data.

## ✨ Features

- Vector embeddings generation for text
- Hybrid search capabilities (text + vector)
- RAG-based context retrieval
- AI chat completions with optional context
- Citation tracking for AI responses
- Generic design supporting custom document types
- Comprehensive error handling and logging

## 🧩 Services

### OpenAI Service

The `OpenAIService` is responsible for generating embeddings from text using Azure OpenAI or OpenAI services. It converts text into vector representations that can be used for semantic search.

#### Key Methods:

- `GetEmbeddingAsync(string text)` - Returns a float array representing the text embedding
- `GetEmbeddingListAsync(string text)` - Returns a List<float> representation of the embedding (for backward compatibility)

```csharp
// Example: Create an OpenAI service and get embeddings
var openAIService = new OpenAIService(configuration, httpClientFactory);
float[] embedding = await openAIService.GetEmbeddingAsync("What are the benefits of RAG?");
```

### AI Search Service

The `AISearchService<T>` provides functionality for uploading documents to and searching within Azure AI Search. It supports hybrid search combining both traditional text search and vector similarity.

#### Key Methods:

- `UploadDocumentsAsync(IEnumerable<T> documents)` - Uploads documents to the search index
- `HybridSearchAsync(string query, float[] embedding, int top = 3)` - Performs a hybrid search with text and vector

```csharp
// Example: Create a search service and perform a search
var searchService = new AISearchService<ProjectDocument>(configuration, logger);
var embedding = await openAIService.GetEmbeddingAsync(query);
var searchResults = await searchService.HybridSearchAsync(query, embedding, 5);
```

### RAG Context Service

The `RagContextService<T>` combines the OpenAI and AI Search services to provide context and citations for RAG applications. It retrieves relevant documents based on user queries and formats them for use with language models.

#### Key Methods:

- `GetContextWithCitationsAsync(string prompt, int top)` - Retrieves relevant context and citations for a prompt

```csharp
// Example: Get context for a user query
var ragService = new RagContextService<ProjectDocument>(openAIService, searchService);
var context = await ragService.GetContextWithCitationsAsync("How do I implement RAG in my app?", 3);
```

### Semantic Kernel Service

The `SemanticKernelService<T>` orchestrates the entire RAG flow, leveraging Microsoft's Semantic Kernel to provide powerful chat completions with or without retrieved context.

#### Key Methods:

- `GetChatCompletionAsync(string prompt, string modelName)` - Gets an AI completion for a prompt
- `GetChatCompletionAsync(string prompt, string modelName, string? context)` - Gets an AI completion with provided context
- `GetRagChatCompletionAsync(string prompt, string modelName, int top = 3)` - Gets a RAG-based completion
- `GetRagChatCompletionWithCitationsAsync(string prompt, string modelName, int top = 3)` - Gets a RAG-based completion with citations

```csharp
// Example: Get a RAG-powered completion with citations
var skService = new SemanticKernelService<ProjectDocument>(
    configuration, logger, searchService, openAIService, ragService);
var (answer, citations) = await skService.GetRagChatCompletionWithCitationsAsync(
    "Explain how to implement hybrid search", "gpt-4", 5);
```

## 🚀 Getting Started

### Installation

1. Add AIServices as a reference in your .NET project:

```xml
<ProjectReference Include="..\AIServices\AIServices.csproj" />
```

### Dependencies

- .NET 8.0 or higher
- Azure.AI.OpenAI
- Azure.Search.Documents
- Microsoft.SemanticKernel

## ⚙️ Configuration

AIServices requires the following configuration settings:

```json
{
	"OPENAI_ENDPOINT": "https://your-openai-service.openai.azure.com/",
	"OPENAI_API_KEY": "your-openai-api-key",
	"OPENAI_EMBEDDING_DEPLOYMENT": "text-embedding-ada-002",
	"OPENAI_CHAT_DEPLOYMENT": "gpt-4",
	"OPENAI_API_VERSION": "2024-04-01-preview",
	"AZURE_SEARCH_ENDPOINT": "https://your-search-service.search.windows.net",
	"AZURE_SEARCH_INDEX": "your-index-name",
	"AZURE_SEARCH_API_KEY": "your-search-api-key"
}
```

You can provide these settings via:

1. Configuration files (appsettings.json)
2. Environment variables
3. Direct parameter initialization

## 📝 Usage Examples

### Basic Chat Completion

```csharp
// Initialize services
var openAIService = new OpenAIService(configuration, httpClientFactory);
var skService = new SemanticKernelService<ProjectDocument>(
    configuration, logger, null, openAIService, ragContextService);

// Get a basic chat completion
string response = await skService.GetChatCompletionAsync(
    "What is Retrieval-Augmented Generation?", "gpt-4");
```

### RAG-Powered Chat with Citations

```csharp
// Initialize services
var openAIService = new OpenAIService(configuration, httpClientFactory);
var searchService = new AISearchService<ProjectDocument>(configuration, logger);
var ragService = new RagContextService<ProjectDocument>(openAIService, searchService);
var skService = new SemanticKernelService<ProjectDocument>(
    configuration, logger, searchService, openAIService, ragService);

// Get a RAG completion with citations
var (answer, citations) = await skService.GetRagChatCompletionWithCitationsAsync(
    "How do I implement vector search?", "gpt-4", 3);

// Process the answer and citations
Console.WriteLine($"Answer: {answer}");
Console.WriteLine("Citations:");
foreach (var citation in citations)
{
    Console.WriteLine($"- {citation.title}: {citation.date_range}");
}
```

### Upload Documents to Search Index

```csharp
// Initialize services
var searchService = new AISearchService<ProjectDocument>(configuration, logger);

// Create documents with embeddings
var openAIService = new OpenAIService(configuration, httpClientFactory);
var documents = new List<ProjectDocument>();
// ... populate documents

// Generate embeddings for each document
foreach (var doc in documents)
{
    doc.content_vector = await openAIService.GetEmbeddingListAsync(
        $"{doc.title} {doc.description} {doc.raw_text}");
}

// Upload to search index
await searchService.UploadDocumentsAsync(documents);
```

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.
