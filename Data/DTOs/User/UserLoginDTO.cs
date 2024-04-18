using System.ComponentModel.DataAnnotations;

namespace WikiApi.Data.DTOs
{
    public class UserLoginDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}