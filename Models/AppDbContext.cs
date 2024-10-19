using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace sproj.Models;

public class AppDbContext : IdentityDbContext<IdentityUser> {
    public AppDbContext(DbContextOptions options) : base(options) { }
}