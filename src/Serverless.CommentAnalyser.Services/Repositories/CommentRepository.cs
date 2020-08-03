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
    public class CommentRepository : BaseRepository, ICommentRepository
    {
        private const int NumberOfPartitions = 5;
        private ILogger<ICommentRepository> _logger;

        public CommentRepository(string connectionString, ILogger<ICommentRepository> logger): base("Comments", connectionString)
        {
            _logger = logger;
        }

        public async Task<CommentEntity> GetCommentAsync(int commentId)
        {
            try
            {
                var partitionKey = GetPartitionKey(commentId);

                var searchOperation = TableOperation.Retrieve<CommentEntity>(partitionKey, commentId.ToString());

                var result = await _table.ExecuteAsync(searchOperation);

                return result.Result as CommentEntity;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);

                throw;
            }
        }

        public async Task SaveCommentAsync(CommentEntity comment)
        {
            if (comment == null)
            {
                throw new ArgumentNullException("Comment is invalid");
            }

            try
            {
                comment.PartitionKey = GetPartitionKey(comment.RowKey);

                var insertOrMergeOperation = TableOperation.InsertOrMerge(comment);

                await _table.ExecuteAsync(insertOrMergeOperation);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);

                throw;
            }
        }

        private string GetPartitionKey(string key)
        {
            return GetPartitionKey(int.Parse(key));
        }

        private string GetPartitionKey(int key)
        {
            return (Math.Abs(key) % NumberOfPartitions).ToString();
        }
    }
}
