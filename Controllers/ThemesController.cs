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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")] // Only admins can manage themes
    public class ThemesController : ControllerBase
    {
        private readonly IThemeRepository _themeRepository;

        public ThemesController(IThemeRepository articleRepository)
        {
            _themeRepository = articleRepository;
        }

        /// <summary>
        /// Get all themes (Accessible only for admins)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetThemes()
        {
            return Ok(await _themeRepository.GetThemes());
        }

        /// <summary>
        /// Get a theme by its Id (Accessible only for admins)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetTheme(int id)
        {
            Theme? existingTheme = await _themeRepository.GetTheme(id);

            return existingTheme is not null ? Ok(existingTheme) : NotFound(); 
        }

        /// <summary>
        /// Create a new theme (Accessible only for admins)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateTheme(ThemeDTOForCreation themeData)
        {
            try
            {
                await _themeRepository.CreateTheme(themeData);
                return Ok("Theme created successfully");
            }
            catch (Exception)
            {
                return Problem("Unable to create theme");
            }
            
           
        }

        /// <summary>
        /// Delete a theme (Accessible only for admins)
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteTheme(int id)
        {
            try
            {
                await _themeRepository.DeleteTheme(id);
                return Ok("Theme deleted successfully");
            }
            catch (Exception)
            {
                return Problem("Unable to delete theme");
            }
        }


        /// <summary>
        /// Update a theme (Accessible only for admins)
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateTheme(int id, ThemeDTOForCreation theme)
        {
            try
            {
                await _themeRepository.UpdateTheme(id, theme);
                return Ok("Theme updated successfully");
            }
            catch (Exception)
            {
                return Problem("Unable to update theme");
            }
           
        }
    }
}