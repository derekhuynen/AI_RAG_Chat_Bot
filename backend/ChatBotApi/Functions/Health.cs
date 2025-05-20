using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ChatBotApi.Functions
{
    /// <summary>
    /// Health check endpoint for the Azure Function app.
    /// Used to verify that the service is running correctly.
    /// </summary>
    public class Health
    {
        private readonly ILogger<Health> _logger;

        public Health(ILogger<Health> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Function("Health")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("Health check endpoint processed a request.");
            return new OkObjectResult("The RAG Chat Bot API is running!");
        }
    }
}