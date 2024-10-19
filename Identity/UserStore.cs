using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace sproj.Identity;

public class UserStore : IUserStore<IdentityUser> {
    private readonly IdentityDbContext _context;

    public UserStore(IdentityDbContext context) {
        _context = context;
    }

    public Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(user);

        return Task.FromResult(user.Id.ToString());
    }

    public Task<string?> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(user);

        return Task.FromResult<string?>(user.UserName);
    }

    public Task<string?> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken) {
        return GetUserNameAsync(user, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(user);

        return Task.FromResult<string?>(user.NormalizedUserName);
    }

    public Task SetUserNameAsync(IdentityUser user, string? userName, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(userName);

        user.UserName = userName;
        return Task.CompletedTask;
    }

    public Task SetNormalizedUserNameAsync(IdentityUser user, string? normalizedName,
        CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(normalizedName);

        user.NormalizedUserName = normalizedName;
        return Task.CompletedTask;
    }

    public async Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(user);

        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(user);

        _context.Users.Update(user);
        try {
            await _context.SaveChangesAsync(cancellationToken);
        } catch (DbUpdateConcurrencyException) {
            return IdentityResult.Failed(new IdentityError { Description = "Concurrency failure." });
        }

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(user);

        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
        return IdentityResult.Success;
    }

    public async Task<IdentityUser?> FindByIdAsync(string userId, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(userId);

        return await _context.Users
            .Where(u => u.Id.ToString() == userId)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IdentityUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(normalizedUserName);

        return await _context.Users
            .Where(u => u.NormalizedUserName == normalizedUserName)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public void Dispose() { }
}