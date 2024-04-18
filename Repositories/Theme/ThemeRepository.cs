using Microsoft.EntityFrameworkCore;
using WikiApi.Data;
using WikiApi.Data.DTOs;
using WikiApi.Data.Models;

namespace WikiApi.Repositories
{
    public class ThemeRepository : IThemeRepository
    {
        private readonly WikiDbContext wikiDbContext;
        public ThemeRepository(WikiDbContext wikiDbContext)
        {
            this.wikiDbContext = wikiDbContext;

        }

        public async Task<Theme> CreateTheme(ThemeDTOForCreation themeData)
        {
            Theme theme = new Theme
            {
                Name = themeData.Name,
            };

            await wikiDbContext.Themes.AddAsync(theme);
            await wikiDbContext.SaveChangesAsync();

            return theme;
        }

        public async Task<Theme> GetTheme(int id) => await wikiDbContext.Themes.FirstOrDefaultAsync(t => t.ThemeId == id);

        public async Task<List<Theme>> GetThemes() => await wikiDbContext.Themes.ToListAsync();

        public async Task UpdateTheme(int id, ThemeDTOForCreation themeData)
        {
            Theme? existingTheme = await wikiDbContext.Themes.FirstOrDefaultAsync(t => t.ThemeId == id);

            if (existingTheme is not null)
            {
                existingTheme.Name = themeData.Name;
                await wikiDbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Theme not found");
            }


        }

        public async Task DeleteTheme(int id)
        {
            Theme? existingTheme = await wikiDbContext.Themes.FirstOrDefaultAsync(a => a.ThemeId == id);

            if (existingTheme is not null)
            {
                wikiDbContext.Themes.Remove(existingTheme);
                await wikiDbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Theme not found");
            }
        }
    }
}