using Microsoft.EntityFrameworkCore;

namespace JWTAuthentication.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
       : base(options)
        {
        }
    }
}
