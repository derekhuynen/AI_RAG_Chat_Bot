# RAG Chat Bot API

![Azure Functions](https://img.shields.io/badge/Azure%20Functions-0062AD?style=for-the-badge&logo=azure-functions&logoColor=white)
![.NET 8](https://img.shields.io/badge/.NET%208-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![OpenAI](https://img.shields.io/badge/OpenAI-412991?style=for-the-badge&logo=openai&logoColor=white)

A powerful API service built on Azure Functions that delivers AI-powered chat responses using Azure OpenAI and Azure AI Search, supporting both standard chat completions and Retrieval-Augmented Generation (RAG) for context-aware responses with citations.

## 📋 Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Features](#features)
- [Endpoints](#endpoints)
  - [Health Check](#health-check)
  - [Chat Completion](#chat-completion)
  - [RAG Chat Completion](#rag-chat-completion)
- [API Usage Examples](#api-usage-examples)
  - [cURL Examples](#curl-examples)
  - [C# Examples](#c-examples)
  - [JavaScript Examples](#javascript-examples)
- [Configuration](#configuration)
- [Deployment](#deployment)
- [Enhancement Opportunities](#enhancement-opportunities)
- [License](#license)

## 🔍 Overview

The RAG Chat Bot API is a .NET-based API service that leverages Azure Functions to provide intelligent chat capabilities. It uses Azure OpenAI for generating responses and Azure AI Search for implementing Retrieval-Augmented Generation (RAG) functionality, allowing the AI to reference a custom knowledge base when answering questions.

## 🏗️ Architecture

The service is built using:

- **Azure Functions**: Serverless compute platform for API endpoints
- **Azure OpenAI**: For AI chat completions and text embeddings
- **Azure AI Search**: Hybrid vector and keyword search for knowledge retrieval
- **Microsoft Semantic Kernel**: Framework for AI orchestration

The API serves as a bridge between client applications and the underlying AI services, handling request validation, context retrieval, and response formatting.

## ✨ Features

- **Health Check Endpoint**: Verify API availability
- **Standard Chat Completion**: Direct chat with the Azure OpenAI service (GPT models)
- **RAG Chat Completion**: Enhanced chat with contextual information retrieved from your document store
- **Citations**: Source references for information provided in RAG responses
- **Error Handling**: Comprehensive error management and logging
- **Azure Functions Integration**: Serverless architecture for scalability and cost efficiency

## 🔌 Endpoints

### Health Check

Verifies that the API service is running correctly.

- **URL**: `/api/Health`
- **Method**: `GET` or `POST`
- **Authorization**: None (Anonymous)
- **Response**:
  - **200 OK**: Service is running

```json
"The RAG Chat Bot API is running!"
```

### Chat Completion

Provides standard chat completions using Azure OpenAI (no context retrieval).

- **URL**: `/api/ChatFunction`
- **Method**: `POST`
- **Authorization**: Function-level
- **Request Body**:

```json
{
	"prompt": "Tell me about artificial intelligence."
}
```

- **Response**:
  - **200 OK**: Successful response

```json
{
	"response": "Artificial Intelligence (AI) refers to computer systems designed to perform tasks that typically require human intelligence..."
}
```

- **400 Bad Request**: Invalid request
- **500 Internal Server Error**: Server-side error

### RAG Chat Completion

Provides chat completions enhanced with relevant information retrieved from your document store.

- **URL**: `/api/RAGChatFunction`
- **Method**: `POST`
- **Authorization**: Function-level
- **Request Body**:

```json
{
	"prompt": "Explain the advantages of using Azure Functions."
}
```

- **Response**:
  - **200 OK**: Successful response with citations

```json
{
	"response": "Azure Functions offers several advantages including serverless architecture, automatic scaling, and pay-per-execution pricing...",
	"citations": [
		{
			"id": "project-123",
			"title": "Serverless Computing with Azure",
			"description": "A deep dive into Azure serverless technologies",
			"tech_stack": ["Azure Functions", "C#", ".NET"],
			"date_range": "2023-2024",
			"metadata": "Additional project information"
		}
	]
}
```

- **400 Bad Request**: Invalid request
- **500 Internal Server Error**: Server-side error

## 📝 API Usage Examples

### cURL Examples

#### Health Check

```bash
curl -X GET "https://your-function-app.azurewebsites.net/api/Health" -H "Content-Type: application/json"
```

#### Chat Completion

```bash
curl -X POST "https://your-function-app.azurewebsites.net/api/ChatFunction?code=your-function-code" \
  -H "Content-Type: application/json" \
  -d '{"prompt":"What is Azure Functions?"}'
```

#### RAG Chat Completion

```bash
curl -X POST "https://your-function-app.azurewebsites.net/api/RAGChatFunction?code=your-function-code" \
  -H "Content-Type: application/json" \
  -d '{"prompt":"Tell me about the benefits of using Azure AI Search with RAG."}'
```

### C# Examples

#### Using HttpClient

```csharp
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public async Task<string> MakeRagChatRequest(string prompt)
{
    using var client = new HttpClient();
    var requestBody = JsonSerializer.Serialize(new { prompt });
    var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

    var response = await client.PostAsync(
        "https://your-function-app.azurewebsites.net/api/RAGChatFunction?code=your-function-code",
        content);

    response.EnsureSuccessStatusCode();
    return await response.Content.ReadAsStringAsync();
}
```

### JavaScript Examples

#### Using Fetch API

```javascript
async function makeRagChatRequest(prompt) {
	const response = await fetch(
		'https://your-function-app.azurewebsites.net/api/RAGChatFunction?code=your-function-code',
		{
			method: 'POST',
			headers: {
				'Content-Type': 'application/json',
			},
			body: JSON.stringify({ prompt }),
		}
	);

	if (!response.ok) {
		throw new Error(`Error: ${response.status}`);
	}

	const data = await response.json();
	return data;
}

// Usage
makeRagChatRequest('What are the key benefits of RAG?')
	.then(data => {
		console.log('Response:', data.response);
		console.log('Citations:', data.citations);
	})
	.catch(error => console.error('Error:', error));
```

## ⚙️ Configuration

The API requires the following configuration settings:

```json
{
	"OPENAI_ENDPOINT": "https://your-openai-service.openai.azure.com/",
	"OPENAI_API_KEY": "your-openai-api-key",
	"OPENAI_API_VERSION": "2024-04-01-preview",
	"OPENAI_EMBEDDING_DEPLOYMENT": "text-embedding-ada-002",
	"OPENAI_CHAT_DEPLOYMENT": "gpt-4.1",
	"AZURE_SEARCH_ENDPOINT": "https://your-search-service.search.windows.net/",
	"AZURE_SEARCH_INDEX": "your-index-name",
	"AZURE_SEARCH_API_KEY": "your-search-api-key"
}
```

In a production environment, it's recommended to:

1. Store secrets in Azure Key Vault
2. Use Managed Identities for secure access to Azure resources
3. Configure proper CORS settings when integrating with web applications

## 🚀 Deployment

### Prerequisites

- .NET 8.0 SDK
- Azure subscription
- Azure Function App (consumption or premium plan)
- Azure OpenAI service instance
- Azure AI Search service instance

### Deployment Steps

1. Build the project:

   ```powershell
   dotnet build --configuration Release
   ```

2. Publish the project:

   ```powershell
   dotnet publish --configuration Release
   ```

3. Deploy to Azure:
   ```powershell
   func azure functionapp publish your-function-app-name
   ```

## 🚩 Enhancement Opportunities

The API can be further enhanced with the following features:

### 💬 Conversation History Support

Implement conversation history to maintain context across multiple interactions:

```json
{
	"prompt": "Tell me more about that.",
	"conversation_id": "conv123",
	"messages": [
		{
			"role": "user",
			"content": "What is Azure Functions?"
		},
		{
			"role": "assistant",
			"content": "Azure Functions is a serverless compute service..."
		}
	]
}
```

### 🔄 Streaming Responses

Add support for streaming responses for a more interactive experience:

```csharp
[Function("StreamingChatFunction")]
public async Task<IActionResult> RunStreamingChat(
    [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
{
    // Setup streaming response
    req.HttpContext.Response.ContentType = "text/event-stream";

    // Stream response chunks as they arrive
    await foreach (var chunk in _semanticKernel.GetStreamingChatCompletionAsync(...))
    {
        await req.HttpContext.Response.WriteAsync($"data: {chunk}\n\n");
        await req.HttpContext.Response.Body.FlushAsync();
    }

    return new EmptyResult();
}
```

### 🔍 Advanced Search Capabilities

Enhance the RAG functionality with:

- Multi-vector search
- Reranking of search results
- Filtering by metadata
- Custom search strategies

### 🔧 User Feedback Loop

Add feedback collection to improve RAG responses over time:

```json
{
	"conversation_id": "conv123",
	"message_id": "msg456",
	"feedback": {
		"rating": 4,
		"comment": "Good response but missed part of my question",
		"was_helpful": true
	}
}
```

### 🔐 Enhanced Security

Implement additional security features:

- Token-based authorization
- Rate limiting
- Request validation middleware
- Content filtering

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.
