using System.ComponentModel.DataAnnotations;

namespace sproj.Data.Entities;

public enum UserRole {
    Employer,
    Worker
}

public class User {
    public int Id { get; set; }
    [MaxLength(256)] public required string FirstName { get; set; }
    [MaxLength(256)] public required string LastName { get; set; }
    [MaxLength(256)] public required string Password { get; set; }
    [MaxLength(15)] public required string PhoneNumber { get; set; }
    [MaxLength(256)] public string Address {get; set; }
    [DataType(DataType.Date)] public DateTime BirthDate { get; set; }
    [MaxLength(15)] public string? CNIC { get; set; }
    [MaxLength(32)] public string? DriverLicense { get; set; }
    public UserRole Role { get; set; }
    public bool IsPhoneVerified { get; set; }
}
