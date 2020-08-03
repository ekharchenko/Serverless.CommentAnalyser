

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Serverless.CommentAnalyser.Services.Abstractions;
using Serverless.CommentAnalyser.Services.Models;
using System;
using System.Threading.Tasks;

namespace Serverless.CommentAnalyser.AzureFunctions.Workers
{
    public class CommentProcessor
    {
        private readonly IJobService _service;
        public CommentProcessor(IJobService service)
        {
            _service = service;
        }

        [FunctionName("CommentProcessor")]
        public async Task Process([QueueTrigger("jobs", Connection = "AzureWebJobsStorage")] string jobId, ILogger log)
        {
            await _service.ProcessAsync(Guid.Parse(jobId));
        }
    }
}
