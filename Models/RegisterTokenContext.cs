using Microsoft.EntityFrameworkCore;

namespace TaskConsumerAPI.Models
{
    public class RegisterTokenContext : DbContext
    {
        public RegisterTokenContext(DbContextOptions<RegisterTokenContext> options) : base(options) { }
        public DbSet<RegisterToken> RegisterTokens { get; set; } = null!;
    }
}
