using auth_playground.Entities;
using Microsoft.EntityFrameworkCore;

namespace auth_playground.Helpers;

public class DataContext: DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Article> Articles { get; set; }
    private readonly IConfiguration Configuration;
    public DataContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
    }
}