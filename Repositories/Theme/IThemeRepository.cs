using WikiApi.Data.DTOs;
using WikiApi.Data.Models;


namespace WikiApi.Repositories
{
    public interface IThemeRepository
    {
        Task<List<Theme>> GetThemes();
        Task<Theme> GetTheme(int id);
        Task<Theme> CreateTheme(ThemeDTOForCreation themeData);
        Task UpdateTheme(int id, ThemeDTOForCreation themeData);
        Task DeleteTheme(int id);
    }
}