using GodzillaLocalFilmes.Movies;
using GodzillaLocalFilmes.Users;
using Microsoft.EntityFrameworkCore;

namespace GodzillaLocalFilmes.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(connectionString: "Data Source=Banco.sqlite");
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
            optionsBuilder.EnableSensitiveDataLogging();

            base.OnConfiguring(optionsBuilder);
        }
    }
}
