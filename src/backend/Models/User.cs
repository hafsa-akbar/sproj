using System.ComponentModel.DataAnnotations;

namespace sproj.Models;

public class User {
    public int Id { get; set; }
    [MaxLength(256)] public required string Username { get; set; }
    [MaxLength(256)] public required string Password { get; set; }

    [MaxLength(15)] public required string PhoneNumber { get; set; }
    public bool IsPhoneVerified { get; set; }
}
