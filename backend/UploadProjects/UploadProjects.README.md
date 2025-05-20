# UploadProjects Tool

![Azure AI Search](https://img.shields.io/badge/Azure%20AI%20Search-0078D4?style=for-the-badge&logo=microsoft-azure&logoColor=white)
![Azure OpenAI](https://img.shields.io/badge/Azure%20OpenAI-412991?style=for-the-badge&logo=openai&logoColor=white)
![.NET 8](https://img.shields.io/badge/.NET%208-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)

A specialized utility for embedding and uploading project documents to Azure AI Search, creating the knowledge base foundation for Retrieval-Augmented Generation (RAG) in the AI RAG Chat Bot system.

## 📋 Table of Contents

- [Overview](#overview)
- [Why It's Essential for RAG](#why-its-essential-for-rag)
- [How It Works](#how-it-works)
- [Project Document Structure](#project-document-structure)
- [Configuration](#configuration)
- [Usage](#usage)
- [Customization](#customization)
- [Best Practices](#best-practices)
- [License](#license)

## 🔍 Overview

The UploadProjects tool is a dedicated command-line utility that processes project documents, generates vector embeddings using Azure OpenAI, and indexes them in Azure AI Search. This creates the searchable knowledge base that powers the Retrieval-Augmented Generation (RAG) functionality in the AI RAG Chat Bot system.

## 🚀 Why It's Essential for RAG

Retrieval-Augmented Generation (RAG) combines the power of large language models with a custom knowledge base to provide accurate, contextually relevant responses. The UploadProjects tool plays a critical role in the RAG architecture by:

1. **Creating the Knowledge Foundation**: It transforms raw project data into searchable documents with vector embeddings.

2. **Enabling Semantic Search**: By generating vector embeddings for each document, it allows the RAG system to find contextually similar content, not just keyword matches.

3. **Maintaining Data Freshness**: It provides a mechanism to update the knowledge base with new project information.

4. **Structuring Project Metadata**: It ensures all projects have a consistent structure with appropriate metadata for filtering and citation.

Without this tool, the RAG system would lack its essential knowledge base, making it impossible to provide context-aware, reliable responses based on your project data.

## ⚙️ How It Works

The UploadProjects tool follows a streamlined process:

1. **Load Configuration**: Reads settings from `local.settings.json` to establish connections to Azure services.

2. **Read Projects**: Loads project documents from the `projects.json` file.

3. **Generate Embeddings**: For each project, it generates vector embeddings using Azure OpenAI's text-embedding models.

4. **Upload to Azure AI Search**: It uploads the embedded documents to Azure AI Search, creating or updating the vector search index.

This process transforms raw project descriptions into searchable, AI-ready documents that power the RAG system's context retrieval capabilities.

## 📄 Project Document Structure

The tool processes project documents with the following structure:

```json
{
	"id": "unique-project-id",
	"title": "Project Title",
	"description": "Brief project description",
	"tech_stack": ["Technology 1", "Technology 2"],
	"date_range": "Jan 2025 - May 2025",
	"metadata": "Additional information",
	"raw_text": "Detailed project description and achievements",
	"content_vector": null // This will be filled by the tool
}
```

Each field serves a specific purpose in the RAG system:

- `id`: Unique identifier for referencing the document
- `title` and `description`: Structured information for display
- `tech_stack`: Categorization for filtering
- `date_range`: Temporal information
- `metadata`: Additional structured data
- `raw_text`: The full content for embedding and retrieval
- `content_vector`: Vector representation for semantic search (generated automatically)

## ⚙️ Configuration

The tool requires the following configuration in `local.settings.json`:

```json
{
	"Values": {
		"OPENAI_ENDPOINT": "https://your-openai-service.openai.azure.com/",
		"OPENAI_API_KEY": "your-openai-api-key",
		"OPENAI_EMBEDDING_DEPLOYMENT": "text-embedding-ada-002",
		"AZURE_SEARCH_ENDPOINT": "https://your-search-service.search.windows.net/",
		"AZURE_SEARCH_INDEX": "your-index-name",
		"AZURE_SEARCH_API_KEY": "your-search-api-key"
	}
}
```

In a production environment, it's recommended to:

1. Use Azure Key Vault for storing secrets
2. Implement Managed Identities for secure access to Azure resources
3. Set up appropriate RBAC permissions

## 📋 Usage

### Prerequisites

- .NET 8.0 SDK
- Azure OpenAI service instance
- Azure AI Search service with a properly configured vector index
- Project documents in JSON format

### Running the Tool

1. Ensure your `projects.json` file contains the projects you want to index
2. Configure your `local.settings.json` with appropriate credentials
3. Run the tool:

```powershell
dotnet run
```

The tool will:

- Load the project documents
- Generate embeddings for each document
- Upload them to your Azure AI Search index
- Report success and failure counts

### Sample Output

```
Loading projects from projects.json...
Loaded 6 projects
Starting embedding and upload process...
Processing project 1/6: p1 - AI for Safety Document Automation
Processing project 2/6: p2 - Regulatory Compliance Chatbot (Azure GenAI POC)
Processing project 3/6: p3 - AI Contract Review Proof of Concept
Processing project 4/6: p4 - AI Book Search & Recommendation Platform
Processing project 5/6: p5 - Nurse Staffing Platform
Processing project 6/6: p6 - Credentialing Portal Project
Uploading 6 documents to Azure AI Search...
Upload complete. Success: 6, Failed: 0
Process completed. Successfully processed: 6, Failed: 0
```

## 🔧 Customization

The UploadProjects tool can be customized to handle different document structures and content sources:

### Supporting Different Document Types

Modify the `ProjectDocument` class in the Model folder to match your document structure.

### Using Alternative Data Sources

Adapt the program to load documents from:

- Databases
- External APIs
- File systems
- Web scraping

### Enhancing the Embedding Process

Improve embeddings by:

- Implementing chunking for long documents
- Adding preprocessing for better semantic representation
- Supporting batch embedding for efficiency

## 💡 Best Practices

For optimal RAG system performance:

1. **Quality Content**: Ensure your project documents contain rich, detailed information.

2. **Consistent Structure**: Maintain a consistent structure across all documents.

3. **Regular Updates**: Refresh your knowledge base as new information becomes available.

4. **Optimized Embedding**: Experiment with different chunking and preprocessing strategies.

5. **Security**: Implement proper credential management using Azure Key Vault and Managed Identities.

6. **Monitoring**: Add telemetry to track embedding and indexing performance.

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.
