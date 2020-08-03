using Serverless.CommentAnalyser.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Serverless.CommentAnalyser.Services.Abstractions
{
    public interface IJobService
    {
        Task ProcessAsync(Guid entry);
    }
}
