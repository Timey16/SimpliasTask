using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace backend.Entities.Contexts
{
    public class UserEntityDbContext : IdentityDbContext<UserEntity>
    {
        public UserEntityDbContext(DbContextOptions<UserEntityDbContext> options) : base(options)
        {

        }
    }
}
