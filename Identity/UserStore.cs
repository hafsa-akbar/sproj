using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace sproj.Identity;

public class UserStore : IUserStore<IdentityUser>, IQueryableUserStore<IdentityUser>, IUserRoleStore<IdentityUser>,
    IUserClaimStore<IdentityUser>, IUserPasswordStore<IdentityUser>, IUserPhoneNumberStore<IdentityUser> {
    private readonly IdentityDbContext _context;

    public UserStore(IdentityDbContext context) {
        _context = context;
    }

    public IQueryable<IdentityUser> Users => _context.Users;

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

    public async Task AddToRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(roleName);

        IdentityRole role = await _context.Roles.SingleOrDefaultAsync(r => r.Name == roleName, cancellationToken) ??
                            throw new InvalidOperationException($"Role '{roleName}' not found.");

        var userRole = new IdentityUserRole {
            UserId = user.Id,
            RoleId = role.Id
        };

        await _context.UserRoles.AddAsync(userRole, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveFromRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(roleName);

        IdentityRole? role = await _context.Roles.SingleOrDefaultAsync(r => r.Name == roleName, cancellationToken);
        if (role == null) {
            throw new InvalidOperationException($"Role '{roleName}' not found.");
        }

        IdentityUserRole? userRole = await _context.UserRoles
            .SingleOrDefaultAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id, cancellationToken);

        if (userRole != null) {
            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IList<string>> GetRolesAsync(IdentityUser user, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(user);

        var roleIds = await _context.UserRoles
            .Where(ur => ur.UserId == user.Id)
            .Select(ur => ur.RoleId)
            .ToListAsync(cancellationToken);

        var roles = await _context.Roles
            .Where(r => roleIds.Contains(r.Id))
            .Select(r => r.Name)
            .ToListAsync(cancellationToken);

        return roles;
    }

    public async Task<bool> IsInRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(roleName);

        IdentityRole? role = await _context.Roles.SingleOrDefaultAsync(r => r.Name == roleName, cancellationToken);
        if (role == null) {
            throw new InvalidOperationException($"Role '{roleName}' not found.");
        }

        return await _context.UserRoles.AnyAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id, cancellationToken);
    }

    public async Task<IList<IdentityUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(roleName);

        IdentityRole? role = await _context.Roles.SingleOrDefaultAsync(r => r.Name == roleName, cancellationToken);
        if (role == null) {
            throw new InvalidOperationException($"Role '{roleName}' not found.");
        }

        var userIds = await _context.UserRoles
            .Where(ur => ur.RoleId == role.Id)
            .Select(ur => ur.UserId)
            .ToListAsync(cancellationToken);

        return await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IList<Claim>> GetClaimsAsync(IdentityUser user, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(user);

        return await _context.UserClaims
            .Where(uc => uc.UserId == user.Id)
            .Select(uc => new Claim(uc.ClaimType, uc.ClaimValue))
            .ToListAsync(cancellationToken);
    }

    public async Task AddClaimsAsync(IdentityUser user, IEnumerable<Claim> claims,
        CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(claims);

        foreach (Claim claim in claims) {
            var userClaim = new IdentityUserClaim {
                UserId = user.Id,
                ClaimType = claim.Type,
                ClaimValue = claim.Value
            };

            await _context.UserClaims.AddAsync(userClaim, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ReplaceClaimAsync(IdentityUser user, Claim claim, Claim newClaim,
        CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(claim);
        ArgumentNullException.ThrowIfNull(newClaim);

        IdentityUserClaim? userClaim = await _context.UserClaims
            .FirstOrDefaultAsync(
                uc => uc.UserId == user.Id && uc.ClaimType == claim.Type && uc.ClaimValue == claim.Value,
                cancellationToken);

        if (userClaim != null) {
            userClaim.ClaimType = newClaim.Type;
            userClaim.ClaimValue = newClaim.Value;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task RemoveClaimsAsync(IdentityUser user, IEnumerable<Claim> claims,
        CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(claims);

        foreach (Claim claim in claims) {
            IdentityUserClaim? userClaim = await _context.UserClaims
                .FirstOrDefaultAsync(
                    uc => uc.UserId == user.Id && uc.ClaimType == claim.Type && uc.ClaimValue == claim.Value,
                    cancellationToken);

            if (userClaim != null) {
                _context.UserClaims.Remove(userClaim);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IList<IdentityUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(claim);

        return await _context.UserClaims
            .Where(uc => uc.ClaimType == claim.Type && uc.ClaimValue == claim.Value)
            .Select(uc => uc.User)
            .ToListAsync(cancellationToken);
    }

    public Task SetPasswordHashAsync(IdentityUser user, string? passwordHash, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(user);

        user.PasswordHash = passwordHash;
        return Task.CompletedTask;
    }

    public Task<string?> GetPasswordHashAsync(IdentityUser user, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(user);

        return Task.FromResult(user.PasswordHash);
    }

    public Task<bool> HasPasswordAsync(IdentityUser user, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(user);

        return Task.FromResult(user.PasswordHash != null);
    }

    public Task<string?> GetPhoneNumberAsync(IdentityUser user, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(user);

        return Task.FromResult(user.PhoneNumber);
    }

    public Task<bool> GetPhoneNumberConfirmedAsync(IdentityUser user, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(user);

        return Task.FromResult(user.PhoneNumberConfirmed);
    }

    public Task SetPhoneNumberAsync(IdentityUser user, string? phoneNumber, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(user);

        user.PhoneNumber = phoneNumber;
        return Task.CompletedTask;
    }

    public Task SetPhoneNumberConfirmedAsync(IdentityUser user, bool confirmed, CancellationToken cancellationToken) {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(user);

        user.PhoneNumberConfirmed = confirmed;
        return Task.CompletedTask;
    }
}