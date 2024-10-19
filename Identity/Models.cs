using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace sproj.Identity;

public class IdentityUser {
    public Guid Id { get; set; }

    [MaxLength(128)] public string UserName { get; set; }
    [MaxLength(128)] public string NormalizedUserName { get; set; }
    [MaxLength(128)] public string? PasswordHash { get; set; }
    [MaxLength(15)] public string? PhoneNumber { get; set; }
    public bool PhoneNumberConfirmed { get; set; }

    public virtual ICollection<IdentityUserRole> UserRoles { get; set; }
    public virtual ICollection<IdentityUserClaim> UserClaims { get; set; }
}

public class IdentityRole {
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(128)] public string Name { get; set; }
    [MaxLength(128)] public string NormalizedName { get; set; }

    public virtual ICollection<IdentityUserRole> UserRoles { get; set; }
}

[PrimaryKey(nameof(UserId), nameof(RoleId))]
public class IdentityUserRole {
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }

    public virtual IdentityUser User { get; set; }
    public virtual IdentityRole Role { get; set; }
}

public class IdentityUserClaim {
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    [MaxLength(256)] public string ClaimType { get; set; }
    [MaxLength(256)] public string ClaimValue { get; set; }

    public virtual IdentityUser User { get; set; }
}