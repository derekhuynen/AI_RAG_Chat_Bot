# AI RAG Chat Bot

![Azure Functions](https://img.shields.io/badge/Azure%20Functions-0062AD?style=for-the-badge&logo=azure-functions&logoColor=white)
![Azure OpenAI](https://img.shields.io/badge/Azure%20OpenAI-412991?style=for-the-badge&logo=openai&logoColor=white)
![Azure AI Search](https://img.shields.io/badge/Azure%20AI%20Search-0078D4?style=for-the-badge&logo=microsoft-azure&logoColor=white)
![.NET 8](https://img.shields.io/badge/.NET%208-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Semantic Kernel](https://img.shields.io/badge/Semantic%20Kernel-512BD4?style=for-the-badge&logo=microsoft&logoColor=white)

A demonstration project showcasing Retrieval-Augmented Generation (RAG) architecture using Azure OpenAI, Azure AI Search, and Microsoft's Semantic Kernel framework. This project provides a template for building AI applications that combine the power of large language models with your own data.

## 📋 Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Key Components](#key-components)
- [Features](#features)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [Technical Highlights](#technical-highlights)
- [Future Enhancements](#future-enhancements)
- [License](#license)

## 🔍 Overview

The AI RAG Chat Bot is a demonstration project that illustrates how to implement Retrieval-Augmented Generation (RAG) architecture using Azure services. RAG combines the capabilities of large language models with custom knowledge bases, allowing the AI to reference specific information when generating responses.

This solution is designed as a reference implementation and teaching tool for developers looking to build similar AI applications.

## 🏗️ Architecture

The solution follows a modular, multi-layered architecture:

![RAG Architecture](https://raw.githubusercontent.com/yourusername/AI_RAG_Chat_Bot/main/architecture-diagram.png)

1. **Knowledge Base Layer**: Azure AI Search index with vector embeddings
2. **AI Services Layer**: Reusable services for embeddings, search, and AI orchestration
3. **API Layer**: Azure Functions exposing chat endpoints
4. **Data Preparation**: Utilities for preparing and uploading documents to the knowledge base

## 🧩 Key Components

The solution consists of three main projects:

### 1. [AIServices](./backend/AIServices/AiServices.ReadMe.md)

A reusable library providing core RAG functionality:

- Vector embedding generation via Azure OpenAI
- Hybrid search (keyword + vector) with Azure AI Search
- Context retrieval and citation tracking
- AI orchestration via Semantic Kernel

### 2. [ChatBotApi](./backend/ChatBotApi/ChatBotApi.ReadMe.md)

An Azure Functions API exposing:

- Standard chat completion endpoint
- RAG-enhanced chat completion with citations
- Health check endpoint

### 3. [UploadProjects](./backend/UploadProjects/UploadProjects.README.md)

A utility for:

- Processing and embedding documents
- Creating the knowledge base in Azure AI Search
- Maintaining the vector index

## ✨ Features

- **Hybrid Search**: Combines traditional keyword search with vector similarity for improved accuracy
- **Context-Aware Responses**: Enhances AI responses with relevant information from your data
- **Citation Tracking**: Provides references to the source documents used in responses
- **Generic Design**: The core library supports custom document types for various use cases
- **Modular Architecture**: Easily adaptable components for integration into other applications

## 🚀 Getting Started

### Prerequisites

- Azure subscription
- Azure OpenAI service
- Azure AI Search service with vector search capability
- .NET 8 SDK

### Configuration

All projects share a similar configuration structure in `local.settings.json`:

```json
{
	"Values": {
		"OPENAI_ENDPOINT": "https://your-openai-service.openai.azure.com/",
		"OPENAI_API_KEY": "your-openai-api-key",
		"OPENAI_EMBEDDING_DEPLOYMENT": "text-embedding-ada-002",
		"OPENAI_CHAT_DEPLOYMENT": "gpt-4",
		"AZURE_SEARCH_ENDPOINT": "https://your-search-service.search.windows.net",
		"AZURE_SEARCH_INDEX": "your-index-name",
		"AZURE_SEARCH_API_KEY": "your-search-api-key"
	}
}
```

### Setup Steps

1. Clone the repository

   ```
   git clone https://github.com/yourusername/AI_RAG_Chat_Bot.git
   cd AI_RAG_Chat_Bot
   ```

2. Update the configuration in each project's `local.settings.json`

3. Prepare your knowledge base

   ```
   cd backend/UploadProjects
   dotnet run
   ```

4. Start the API

   ```
   cd ../ChatBotApi
   func start
   ```

5. Test the API using the examples provided in the [ChatBotApi README](./backend/ChatBotApi/ChatBotApi.ReadMe.md)

## 📂 Project Structure

```
backend/
├── AIRAGChatBot.sln            # Solution file
├── AIServices/                 # Core RAG library
│   ├── Model/                  # Document models
│   └── Service/                # Service implementations
│       └── Interface/          # Service interfaces
├── ChatBotApi/                 # Azure Functions API
│   ├── Functions/              # API endpoints
│   └── Model/                  # API models
└── UploadProjects/             # Knowledge base utility
    ├── Model/                  # Document models
    └── Service/                # Service implementations
```

## 💡 Technical Highlights

### Semantic Kernel Integration

The solution leverages Microsoft's [Semantic Kernel](https://github.com/microsoft/semantic-kernel) framework to orchestrate AI interactions:

- **Simplified AI Orchestration**: Semantic Kernel abstracts the complexity of working with multiple Azure AI services
- **Prompt Management**: Consistent handling of system and user prompts
- **Context Awareness**: Seamless integration of retrieved context into the AI's conversation flow
- **Extensibility**: The Kernel-based design allows for easy addition of plugins and skills

### Advanced RAG Techniques

- **Hybrid Search**: Combines BM25 text search with vector similarity for more accurate retrieval
- **Citation Management**: Tracks and returns source documents for verification and transparency
- **Generic Document Support**: Core services use generics to support different document types

### Azure Integration

- **Serverless Design**: Azure Functions provides scalable, cost-effective API hosting
- **Azure AI Services**: Leverages Azure OpenAI and Azure AI Search capabilities
- **Configuration Management**: Consistent environment variable approach across components

## 🚩 Future Enhancements

This demo project can be extended with:

- **Conversation History**: Managing context across multiple messages
- **Streaming Responses**: Real-time token streaming for a more interactive experience
- **Advanced Search Features**: Multi-vector search, reranking, and metadata filtering
- **User Feedback Loop**: Collecting feedback to improve future responses
- **Web UI**: A frontend interface for interacting with the chatbot

See the individual component READMEs for more detailed enhancement ideas.

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.
