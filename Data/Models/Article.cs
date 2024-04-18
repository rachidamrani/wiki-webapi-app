using System.ComponentModel.DataAnnotations;

namespace WikiApi.Data.Models
{
    public class Article
    {
        public Guid ArticleId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [MaxLength(100)]
        public required string Title { get; set; }
        public required string Body { get; set; }

        [EnumDataType(typeof(Priority))]
        public required Priority Priority { get; set; }
        public int ThemeId { get; set; }
        public virtual User User { get; set; }
        public virtual List<Comment>? Comments { get; set; }
    }
}