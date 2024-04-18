using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WikiApi.Data.Models;
using WikiApi.DTOs;
using WikiApi.Repositories;

namespace WikiApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, User")]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleRepository _articleRepository;

        public ArticlesController(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        /// <summary>
        /// Get all articles (Accessible for all visitors)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetArticles()
        {
            return Ok(await _articleRepository.GetArticles());
        }

        /// <summary>
        /// Get an article by its Id (Accessible for all visitors)
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetArticle(Guid id)
        {
            Article? existingArticle = await _articleRepository.GetArticle(id);

            if (existingArticle is null) return NotFound();

            return Ok(existingArticle);
        }

        /// <summary>
        /// Create a new article (Only a connected user can create an article)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateArticle(ArticleForCreationDTO articleData)
        {
            string? userId = HttpContext.User.Claims.First().Value;

            try
            {
                await _articleRepository.CreateArticle(articleData, userId);
                return Ok("Article created successfully");
            }
            catch (Exception ex)
            {
                return Problem("Unable to create article");
            }

        }

        /// <summary>
        /// Update an article (Only the creator / admin of an article can edit it)
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArticle(Guid id, ArticleForCreationDTO articleData)
        {
            string? userId = HttpContext.User.Claims.First().Value;

            try
            {
                await _articleRepository.UpdateArticle(id, articleData, userId);
                return Ok("Article updated successfully");
            }
            catch (Exception ex)
            {
                return Problem("Unable to delete article");
            }

        }

        /// <summary>
        /// Delete an article (Only the creator / admin of an article can delete it)
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(Guid id)
        {
            string? userId = HttpContext.User.Claims.First().Value;

            try
            {
                await _articleRepository.DeleteArticle(id, userId);
                return Ok("Article deleted successfully");
            }
            catch (Exception ex)
            {
                return Problem("Unable to delete article");
            }
        }
    }
}
