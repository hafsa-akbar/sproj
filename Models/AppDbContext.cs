using Microsoft.EntityFrameworkCore;
using sproj.Identity;

namespace sproj.Models;

public class AppDbContext : IdentityDbContext {
    public AppDbContext(DbContextOptions options) : base(options) { }
}