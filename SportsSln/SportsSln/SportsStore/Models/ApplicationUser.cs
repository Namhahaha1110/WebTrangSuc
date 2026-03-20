using Microsoft.AspNetCore.Identity;

namespace SportsStore.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? Role { get; set; }
    }
}
