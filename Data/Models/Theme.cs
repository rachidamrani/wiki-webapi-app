namespace WikiApi.Data.Models
{
    public class Theme
    {
        public int ThemeId { get; set; }
        public required string Name { get; set; }
        public virtual List<Article>? Articles { get; set; }
    }
}