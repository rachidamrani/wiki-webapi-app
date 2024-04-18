using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WikiApi.Data;
using WikiApi.Data.Models;
using WikiApi.DTOs;

namespace WikiApi.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly WikiDbContext wikiDbContext;
        private readonly UserManager<User> _userManager;

        public ArticleRepository(WikiDbContext wikiDbContext,
        UserManager<User> userManager)
        {
            this.wikiDbContext = wikiDbContext;
            _userManager = userManager;
        }

        public async Task<Article> CreateArticle(ArticleForCreationDTO articleData, string userId)
        {
            User? user = await _userManager.FindByIdAsync(userId);

            Article article = new()
            {
                ArticleId = Guid.NewGuid(),
                Title = articleData.Title,
                Body = articleData.Body,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Priority = articleData.Priority,
                ThemeId = articleData.ThemeId,
                User = user
            };

            await wikiDbContext.Articles.AddAsync(article);
            await wikiDbContext.SaveChangesAsync();

            return article;
        }

        public async Task<Article> GetArticle(Guid id)
        {
            return await wikiDbContext.Articles.Include(a => a.Comments).FirstOrDefaultAsync(a => a.ArticleId == id);
        }

        public async Task<List<Article>> GetArticles()
        {
            return await wikiDbContext.Articles.Include(a => a.Comments).ToListAsync();
        }

        public async Task UpdateArticle(Guid id, ArticleForCreationDTO articleData, string userId)
        {
            // Check if the connected user is the creator / admin  before updating

            User? user = await _userManager.FindByIdAsync(userId);
            Article? existingArticle = await wikiDbContext.Articles.FirstOrDefaultAsync(a => a.ArticleId == id);

            bool userIsOwner = existingArticle!.User == user;

            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (existingArticle is not null)
            {
                if (userIsOwner || isAdmin)
                {
                    existingArticle.UpdatedAt = DateTime.Now;
                    existingArticle.Title = articleData.Title;
                    existingArticle.Body = articleData.Body;
                    existingArticle.Priority = articleData.Priority;
                    existingArticle.ThemeId = articleData.ThemeId;

                    await wikiDbContext.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Unable to update article");
                }
            }
            else
            {
                throw new Exception("Article not found");
            }




        }

        public async Task DeleteArticle(Guid id, string userId)
        {
            // Check if the connected user is the creator / admin  before deleting
            User? user = await _userManager.FindByIdAsync(userId);
            Article? existingArticle = await wikiDbContext.Articles.FirstOrDefaultAsync(a => a.ArticleId == id);

            bool userIsOwner = existingArticle!.User == user;

            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (existingArticle is not null)
            {
                if (userIsOwner || isAdmin)
                {
                    wikiDbContext.Articles.Remove(existingArticle);
                    await wikiDbContext.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Unable to delete article");
                }
            }
            else
            {
                throw new Exception("Article not found");
            }
        }
    }
}