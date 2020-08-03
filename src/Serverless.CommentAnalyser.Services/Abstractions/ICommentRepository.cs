using Serverless.CommentAnalyser.Services.Models;
using System.Threading.Tasks;

namespace Serverless.CommentAnalyser.Services.Abstractions
{
    public interface ICommentRepository
    {
         Task<CommentEntity> GetCommentAsync(int commentId);

         Task SaveCommentAsync(CommentEntity comment);
    }
}