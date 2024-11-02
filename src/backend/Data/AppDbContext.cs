using Microsoft.EntityFrameworkCore;
using sproj.Data.Entities;

namespace sproj.Data;

public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions options) : base(options) { }

    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        base.OnConfiguring(optionsBuilder.UseLowerCaseNamingConvention());
    }
}
