using Microsoft.EntityFrameworkCore;

namespace YourProjectNamespace.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<TokenInfo> Tokens { get; set; }
        // Define DbSets for your entities, e.g., 
        // public DbSet<YourEntity> YourEntities { get; set; }
    }
}
