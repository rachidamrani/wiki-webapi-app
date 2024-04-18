using WikiApi.Data.Models;
using WikiApi.DTOs;

namespace WikiApi.Repositories
{
    public interface IArticleRepository
    {
        Task<List<Article>> GetArticles();
        Task<Article> GetArticle(Guid id);
        Task<Article> CreateArticle(ArticleForCreationDTO articleData, string userId);
        Task UpdateArticle(Guid id, ArticleForCreationDTO articleData, string userId);
        Task DeleteArticle(Guid id, string userId);
    }
}