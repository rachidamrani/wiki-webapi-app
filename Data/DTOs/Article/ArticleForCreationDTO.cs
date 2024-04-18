using System.ComponentModel.DataAnnotations;
using WikiApi.Data;

namespace WikiApi.DTOs
{
    public class ArticleForCreationDTO
    {

        [MaxLength(100)]
        public required string Title { get; set; }
        public required string Body { get; set; }

        [EnumDataType(typeof(Priority))]
        public required Priority Priority { get; set; }
        public int ThemeId { get; set; }

    }
}