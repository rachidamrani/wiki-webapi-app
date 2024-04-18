using System.ComponentModel.DataAnnotations;

namespace WikiApi.Data.DTOs
{
    public class UserRegistrationDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Birthday { get; set; }

        [Required]
        public string Password { get; set; }
    }
}