using WikiApi.Data.DTOs;
using WikiApi.Data.Models;

namespace WikiApi.Repositories
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetComments();
        Task<Comment> GetComment(Guid id);
        Task<Comment> CreateComment(CommentDTOForCreation commentData,
        Guid articleId, string userId);
        Task UpdateComment(Guid id,
        CommentDTOForCreation commentData, string userId);
        Task DeleteComment(Guid id, string userId);
    }
}