using Microsoft.AspNetCore.Identity;

namespace backend.Entities
{
    public class UserEntity : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
