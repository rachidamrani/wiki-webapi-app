using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WikiApi.Data.Models;

namespace WikiApi.Data
{
    public class WikiDbContext : IdentityDbContext
    {

        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Theme> Themes { get; set; }
        public WikiDbContext(DbContextOptions<WikiDbContext> options) : base(options)
        {

        }
    }
}