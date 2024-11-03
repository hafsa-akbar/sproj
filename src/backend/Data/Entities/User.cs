using System.ComponentModel.DataAnnotations;

namespace sproj.Data.Entities;

public class User {
    public int UserId { get; init; }

    [MaxLength(15)]
    public required string PhoneNumber { get; set; }

    [MaxLength(128)]
    public required string Password { get; set; }

    public bool IsPhoneVerified { get; set; }
    // public bool IsCnicVerified { get; set; }

    public UserDetails? UserDetails { get; set; }
}
