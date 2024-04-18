using Microsoft.AspNetCore.Identity;

namespace WikiApi.Data.Models
{
    public class User : IdentityUser
    {
        public DateOnly Birthday { get; set; }
    }
}