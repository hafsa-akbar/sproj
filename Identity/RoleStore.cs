using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace sproj.Identity;

public class RoleStore : IRoleStore<IdentityRole>, IQueryableRoleStore<IdentityRole> {
    private readonly IdentityDbContext _context;

    public RoleStore(IdentityDbContext context) {
        _context = context;
    }

    public IQueryable<IdentityRole> Roles => _context.Roles;

    public async Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(role);

        await _context.Roles.AddAsync(role, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(role);

        _context.Roles.Update(role);
        await _context.SaveChangesAsync(cancellationToken);
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(role);

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync(cancellationToken);
        return IdentityResult.Success;
    }

    public Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(role);

        return Task.FromResult(role.Id.ToString());
    }

    public Task<string?> GetRoleNameAsync(IdentityRole role, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(role);

        return Task.FromResult<string?>(role.Name);
    }

    public Task SetRoleNameAsync(IdentityRole role, string? roleName, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(role);
        ArgumentNullException.ThrowIfNull(roleName);

        role.Name = roleName;
        return Task.CompletedTask;
    }

    public Task<string?> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(role);

        return Task.FromResult<string?>(role.NormalizedName);
    }

    public Task SetNormalizedRoleNameAsync(IdentityRole role, string? normalizedName,
        CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(role);

        role.NormalizedName = normalizedName;
        return Task.CompletedTask;
    }

    public async Task<IdentityRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(roleId);

        return await _context.Roles.FindAsync(new object[] { Guid.Parse(roleId) }, cancellationToken);
    }

    public async Task<IdentityRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(normalizedRoleName);

        return await _context.Roles
            .SingleOrDefaultAsync(r => r.NormalizedName == normalizedRoleName, cancellationToken);
    }

    public void Dispose() { }
}