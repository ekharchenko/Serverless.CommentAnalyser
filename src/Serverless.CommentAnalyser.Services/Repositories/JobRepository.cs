using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Serverless.CommentAnalyser.Services.Abstractions;
using Serverless.CommentAnalyser.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Serverless.CommentAnalyser.Services.Repositories
{
    public class JobRepository : BaseRepository, IJobRepository
    {
        private const string PartitionKey = "Comment";

        private ILogger<IJobRepository> _logger;

        public JobRepository(string connectionString, ILogger<IJobRepository> logger) : base("Jobs", connectionString)
        {
            _logger = logger;
        }

        public async Task<JobEntity> GetJobAsync(Guid id)
        {
            try
            {
                var searchOperation = TableOperation.Retrieve<JobEntity>(PartitionKey, id.ToString());

                var result = await _table.ExecuteAsync(searchOperation);

                return result.Result as JobEntity;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);

                throw;
            }
        }

        public async Task SaveJobAsync(JobEntity job)
        {
            if (job == null)
            {
                throw new ArgumentNullException("Job is invalid");
            }

            try
            {
                job.PartitionKey = PartitionKey;

                var insertOrMergeOperation = TableOperation.InsertOrMerge(job);

                await _table.ExecuteAsync(insertOrMergeOperation);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);

                throw;
            }
        }
    }
}
