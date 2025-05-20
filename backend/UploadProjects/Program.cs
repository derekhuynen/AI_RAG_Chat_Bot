using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using UploadProjects.Model;
using UploadProjects.Service;

namespace UploadProjects
{
    /// <summary>
    /// Command-line program to embed and upload project documents to Azure AI Search
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            // Load environment variables from local.settings.json
            LoadEnvironmentVariables();

            // Create a direct instance of ProjectUploadService
            var projectService = new ProjectUploadService();

            // Load projects from projects.json
            Console.WriteLine("Loading projects from projects.json...");
            if (!File.Exists("projects.json"))
            {
                Console.WriteLine("Error: projects.json not found in the current directory");
                return;
            }

            var projects = JsonSerializer.Deserialize<List<ProjectDocument>>(File.ReadAllText("projects.json")) ?? new List<ProjectDocument>();
            Console.WriteLine($"Loaded {projects.Count} projects");

            // Use the upload service for embedding and upload
            Console.WriteLine("Starting embedding and upload process...");
            var result = await projectService.EmbedAndUploadAsync(projects);
            Console.WriteLine($"Process completed. Successfully processed: {result.success}, Failed: {result.fail}");
        }

        /// <summary>
        /// Loads environment variables from local.settings.json
        /// </summary>
        private static void LoadEnvironmentVariables()
        {
            try
            {
                if (File.Exists("local.settings.json"))
                {
                    var settings = JsonSerializer.Deserialize<LocalSettings>(File.ReadAllText("local.settings.json"));
                    if (settings?.Values != null)
                    {
                        foreach (var (key, value) in settings.Values)
                        {
                            Environment.SetEnvironmentVariable(key, value);
                        }
                    }
                    Console.WriteLine("Environment variables loaded from local.settings.json");
                }
                else
                {
                    Console.WriteLine("Warning: local.settings.json not found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading settings: {ex.Message}");
            }
        }
    }
}
