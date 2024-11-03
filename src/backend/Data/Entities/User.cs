namespace sproj.Data.Entities;

public enum UserState {
    Unregistered,
    Unverified,
    Employer,
    UnverifiedWorker,
    Worker
}

public class User {
    public int Id { get; init; }

    public required string PhoneNumber { get; set; }
    public required string Password { get; set; }

    public bool IsPhoneVerified { get; set; }
    public UserState State { get; set; } = UserState.Unregistered;

    // public string FirstName { get; set; }
    // public string LastName { get; set; }
    // public string Address { get; set; }
    // public DateTime BirthDate { get; set; }
    // public string? CNIC { get; set; }
    // public string? DriverLicense { get; set; }
}
