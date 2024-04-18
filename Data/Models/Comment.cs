using System.ComponentModel.DataAnnotations;

namespace WikiApi.Data.Models
{
    public class Comment
    {
        public Guid CommentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [MaxLength(100)]
        public string Body { get; set; }
        public virtual User User { get; set; }
        public virtual Article Article { get; set; }
    }
}