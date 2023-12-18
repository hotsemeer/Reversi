using Microsoft.EntityFrameworkCore;
using Reversi_api.Models;

namespace Reversi_api.Data
{
    public class ReversiContext : DbContext
    {
        public ReversiContext(DbContextOptions<ReversiContext> options)
            : base(options)
        {
        }

        public DbSet<Game> Games { get; set; } = default!;

        public DbSet<Player> Player { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>().HasMany(g => g.Columns);
            modelBuilder.Entity<Column>().HasMany(c => c.Rows);
        }
    }
}
