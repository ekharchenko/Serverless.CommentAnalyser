using Serverless.CommentAnalyser.Services.Abstractions;
using Serverless.CommentAnalyser.Services.Enums;
using Serverless.CommentAnalyser.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Serverless.CommentAnalyser.Services
{
    public class JobService : IJobService
    {
        private readonly ITextAnalyzer _textAnalyzer;
        private readonly ICommentRepository _commentRepository;
        private readonly IJobRepository _jobRepository;

        public JobService(ITextAnalyzer textAnalyzer, ICommentRepository commentRepository, IJobRepository jobRepository)
        {
            _textAnalyzer = textAnalyzer;
            _commentRepository = commentRepository;
            _jobRepository = jobRepository;
        }

        public async Task ProcessAsync(Guid entry)
        {
            var job = await _jobRepository.GetJobAsync(entry);
            var comment = job.Content;

            var key = comment.GetHashCode();

            if (await _commentRepository.GetCommentAsync(key) == null)
            {
                var result = await _textAnalyzer.GetCommentToneAsync(comment);

                await _commentRepository.SaveCommentAsync(new CommentEntity { RowKey = key.ToString(),
                                                                            Content = comment,
                                                                            ToneInformation = result });
            }

            job.JobStatus = JobStatus.Finished;
            job.TargetId = key;

            await _jobRepository.SaveJobAsync(job);
        }
    }
}
