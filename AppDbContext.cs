using Microsoft.EntityFrameworkCore;
using sproj.Identity;

namespace sproj;

public class AppDbContext : IdentityDbContext {
    public AppDbContext(DbContextOptions options) : base(options) { }
}