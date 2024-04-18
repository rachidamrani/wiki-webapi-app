using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WikiApi.Data.DTOs;
using WikiApi.Data.Models;
using WikiApi.Repositories;

namespace WikiApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, User")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;

        public CommentsController(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }


        /// <summary>
        /// Get all comments (Accessible for all visitors)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetComments()
        {
            return Ok(await _commentRepository.GetComments());
        }

        /// <summary>
        /// Get a comment by its Id (Accessible for all visitors)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetComment(Guid id)
        {
            Comment comment = await _commentRepository.GetComment(id);
            if (comment is null) return NotFound();

            return Ok(comment);
        }

        /// <summary>
        /// Create a new comment (Only a connected user can create a comment)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateComment(CommentDTOForCreation commentData, Guid articleId)
        {
            string? userId = HttpContext.User.Claims.First().Value;

            try
            {
                await _commentRepository.CreateComment(commentData, articleId, userId);
                return Ok("Created succesffuly");
            }
            catch (Exception ex)
            {
                return Problem("Unable to create comment");
            }
        }

        /// <summary>
        /// Delete a comment (Only the creator / admin of a article can delete it)
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            string? userId = HttpContext.User.Claims.First().Value;

            try
            {
                await _commentRepository.DeleteComment(id, userId);
                return Ok("Comment deleted successfully");
            }
            catch (Exception)
            {
                return Problem("Unable to delete comment");
            }

        }

        /// <summary>
        /// Update a comment (Only the creator / admin of a comment can edit it)
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateComment(Guid id, CommentDTOForCreation commentData)
        {
            string? userId = HttpContext.User.Claims.First().Value;

            try
            {
                await _commentRepository.UpdateComment(id, commentData, userId);
                return Ok("Updated successfully");
            }
            catch (Exception)
            {
                return Problem("Unable to update comment");
            }
        }
    }
}
