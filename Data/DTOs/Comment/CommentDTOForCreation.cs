using System.ComponentModel.DataAnnotations;

namespace WikiApi.Data.DTOs
{
    public class CommentDTOForCreation
    {
        [MaxLength(100)]
        public string Body { get; set; }
    }
}