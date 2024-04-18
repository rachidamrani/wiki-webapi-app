using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WikiApi.Data;
using WikiApi.Data.DTOs;
using WikiApi.Data.Models;

namespace WikiApi.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly WikiDbContext _wikiDbContext;
        private readonly UserManager<User> _userManager;

        public CommentRepository(WikiDbContext wikiDbContext, UserManager<User> userManager)
        {
            _wikiDbContext = wikiDbContext;
            _userManager = userManager;
        }

        public async Task<Comment> CreateComment(CommentDTOForCreation commentData, Guid articleId, string userId)
        {
            Article? article = await _wikiDbContext.Articles.FindAsync(articleId);
            User? user = await _userManager.FindByIdAsync(userId);

            Comment comment = new Comment
            {
                CommentId = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Body = commentData.Body,
                Article = article,
                User = user,
            };

            await _wikiDbContext.Comments.AddAsync(comment);
            await _wikiDbContext.SaveChangesAsync();

            return comment;
        }

        public async Task<Comment> GetComment(Guid id) => await _wikiDbContext.Comments.FindAsync(id);

        public async Task<List<Comment>> GetComments()
        {
            return await _wikiDbContext.Comments.ToListAsync();
        }

        public async Task UpdateComment(Guid id, CommentDTOForCreation commentData, string userId)
        {
            Comment? existingComment = await _wikiDbContext.Comments.FindAsync(id);
            User? user = await _userManager.FindByIdAsync(userId);

            bool userIsOwner = existingComment!.User == user;

            bool userIsAdmin = await _userManager.IsInRoleAsync(user, "Admin");


            if (existingComment is not null)
            {
                if (userIsOwner || userIsAdmin)
                {
                    existingComment.Body = commentData.Body;
                    existingComment.UpdatedAt = DateTime.Now;
                    await _wikiDbContext.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Unable to delete comment");
                }
            }
            else
            {
                throw new Exception("Comment not found");
            }
        }

        public async Task DeleteComment(Guid id, string userId)
        {
            Comment? existingComment = await _wikiDbContext.Comments.FindAsync(id);
            User? user = await _userManager.FindByIdAsync(userId);

            bool userIsOwner = existingComment!.User == user;

            bool userIsAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (existingComment is not null)
            {

                if (userIsOwner || userIsAdmin)
                {
                    _wikiDbContext.Comments.Remove(existingComment);
                    await _wikiDbContext.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Unable to delete comment");
                }
            }
            else
            {
                throw new Exception("Comment not found");
            }
        }
    }
}