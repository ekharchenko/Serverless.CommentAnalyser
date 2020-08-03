using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Serverless.CommentAnalyser.Services.Abstractions
{
    public interface ITextAnalyzer
    {
        Task<string> GetCommentToneAsync(string text);
    }
}
