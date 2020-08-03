using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json.Linq;
using Serverless.CommentAnalyser.AzureFunctions.Models;
using Serverless.CommentAnalyser.Services.Abstractions;
using Serverless.CommentAnalyser.Services.Enums;
using Serverless.CommentAnalyser.Services.Models;

namespace Serverless.CommentAnalyser.AzureFunctions.Endpoints
{
    [ApiExplorerSettings(GroupName = "Comment Analyzer")]
    public class Frontend
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IJobRepository _jobRepository;

        public Frontend(ICommentRepository commentRepository, IJobRepository jobRepository)
        {
            _commentRepository = commentRepository;
            _jobRepository = jobRepository;
        }

        /// <summary>
        /// The method gets a comment by comment id
        /// </summary>
        /// <param name="id"> comment id (integer type)</param>
        /// <response code="200">Returns comment in json</response>
        /// <response code="400">comment id is not a valid integer</response>
        /// <response code="404">A comment does not exist with the current id</response>
        /// <response code="500">Server has an internal error</response>
        [ProducesResponseType(typeof(Comment), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(GenericMessage), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [FunctionName("GetComment")]
        public async Task<IActionResult> GetComment([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/comment/{id:int}")] HttpRequestMessage request, int id)
        {
            var comment = await  _commentRepository.GetCommentAsync(id);

            if(comment == null)
            {
                return new NotFoundObjectResult(new GenericMessage { Message = $"Comment with the id {id} was not found" });
            }
            
            return new OkObjectResult(new Comment {  Id = comment.RowKey, Content = comment.Content, Tone = JObject.Parse(comment.ToneInformation)});
        }

        /// <summary>
        /// The method adds a comment to a pool for comment analysing and stores it to database 
        /// </summary>
        /// <param name="content"> Plain text</param>
        /// <response code="202">Returns  a job status url if the service accepts the comment for processing</response>
        /// <response code="400">Comment is empty or has only whitespaces</response>
        /// <response code="500">Server has an internal error</response>
        [ProducesResponseType(typeof(LocationMessage), (int)HttpStatusCode.Accepted)]
        [ProducesResponseType(typeof(ValidationErrorMessage), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [SupportedRequestFormat("text/plain")]
        [FunctionName("AddComment")]
        public async Task<IActionResult> AddComment([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/comment"),RequestBodyType(typeof(string), "comment text")] string content, [Queue("jobs", Connection = "AzureWebJobsStorage")] IAsyncCollector<string> jobQueue)
        {
            if(string.IsNullOrWhiteSpace(content))
            {
                return new BadRequestObjectResult(new ValidationErrorMessage { Errors = new List<string>() { "The comment can not be empty or null or only whitespaces" } });
            }

            var jobId = Guid.NewGuid();
            
            await _jobRepository.SaveJobAsync(new JobEntity { RowKey = jobId.ToString(), Content = content });

            await jobQueue.AddAsync(jobId.ToString());
     
            return new ObjectResult(new LocationMessage { Location = $"api/v1/job/{jobId}" }) { StatusCode = (int) HttpStatusCode.Accepted};
        }

        /// <summary>
        /// The method gets a job status by job id 
        /// </summary>
        /// <param name="id"> job id </param>
        /// <response code="201">Returns a link to a comment</response>
        /// <response code="202">A job is not finished</response>
        /// <response code="400">Job id is not a valid guid</response>
        /// <response code="404">A job does not exist with the current id</response>
        /// <response code="500">Server has an internal error</response>
        [ProducesResponseType(typeof(LocationMessage), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(GenericMessage), (int)HttpStatusCode.Accepted)]
        [ProducesResponseType(typeof(GenericMessage), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [FunctionName("GetJob")]
        public async Task<IActionResult> GetJob([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/job/{id:guid}")] HttpRequestMessage request, Guid id)
        {
            var job = await _jobRepository.GetJobAsync(id);

            if (job == null)
            {
                return new NotFoundObjectResult(new GenericMessage { Message = $"Job with the id {id} was not found" });
            }

            if (job.JobStatus != JobStatus.Finished)
            {
                return new ObjectResult(new GenericMessage { Message = $"The task with the id {id} is stil in progress" }) { StatusCode = (int)HttpStatusCode.Accepted };
            }
            
            return new ObjectResult(new LocationMessage { Location = $"api/v1/comment/{job.TargetId}" }) { StatusCode = (int)HttpStatusCode.Created };
        }

    }
}