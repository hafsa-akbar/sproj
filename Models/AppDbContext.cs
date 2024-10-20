using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace sproj.Models;

public class AppDbContext : IdentityDbContext<IdentityUser> {
    public AppDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder) {
        base.OnModelCreating(builder);
        
        builder.Entity<IdentityUser>().ToTable("aspnet_users");
        builder.Entity<IdentityRole>().ToTable("aspnet_roles");
        builder.Entity<IdentityUserRole<string>>().ToTable("aspnet_userroles");
        builder.Entity<IdentityUserClaim<string>>().ToTable("aspnet_userclaims");
        builder.Entity<IdentityUserToken<string>>().ToTable("aspnet_usertokens");
        builder.Entity<IdentityUserLogin<string>>().ToTable("aspnet_userlogins");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("aspnet_roleclaims");
    }
}