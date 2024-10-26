using Microsoft.EntityFrameworkCore;

namespace sproj.Models;

public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions options) : base(options) { }

    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        base.OnConfiguring(optionsBuilder.UseLowerCaseNamingConvention());
    }
}