// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength
namespace sproj.Data.Entities;

public class UserDetails {
    public required int UserId { get; set; }
    public User? User { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Address { get; set; }
    public DateTime BirthDate { get; set; }
    public string? Cnic { get; set; }
    public string? DriverLicense { get; set; }
}
