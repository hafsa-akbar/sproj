using Microsoft.EntityFrameworkCore;

namespace sproj.Identity;

public class IdentityDbContext : DbContext {
    public IdentityDbContext(DbContextOptions options) : base(options) { }

    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityUserRole> UserRoles { get; set; }
    public DbSet<IdentityUserClaim> UserClaims { get; set; }
}