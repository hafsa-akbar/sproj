using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace sproj.Data;

public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Job> Jobs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Job>().HasOne(j => j.WorkerDetails).WithOne().HasForeignKey<Job>(j => j.UserId);
        modelBuilder.Entity<PastJob>().HasOne(j => j.WorkerDetails).WithOne().HasForeignKey<PastJob>(j => j.UserId);
    }
}

public class User {
    public int UserId { get; set; }

    [MaxLength(15)] public required string PhoneNumber { get; set; }
    [MaxLength(128)] public required string Password { get; set; }

    public required Role Role { get; set; }

    public int? CoupleUserId { get; set; }
    public User? Couple { get; set; }

    [MaxLength(128)] public required string FullName { get; set; }
    [MaxLength(512)] public required string Address { get; set; }
    public required DateOnly Birthdate { get; set; }
    public required UserGender Gender { get; set; }
    [MaxLength(32)] public string? CnicNumber { get; set; }
    [MaxLength(32)] public string? DrivingLicense { get; set; }

    public UserPreferences? UserPreferences { get; set; }
    public SmsVerification? SmsVerifications { get; set; }
    public CnicVerification? CnicVerifications { get; set; }
    public WorkerDetails? WorkerDetails { get; set; }
}

public class UserPreferences {
    [Key] public int UserId { get; set; }

    public required List<JobCategory> JobCategories { get; set; }
    public required List<JobExperience> JobExperiences { get; set; }
    public required List<JobType> JobTypes { get; set; }
    [MaxLength(64)] public string? JobLocale { get; set; }
}

public class SmsVerification {
    [Key] public int UserId { get; set; }

    [MaxLength(6)] public required string VerificationCode { get; set; }
    public required DateTime ExpiresAt { get; set; }
}

public class CnicVerification {
    [Key] public int UserId { get; set; }

    public required byte[] IdImage { get; set; }
    public required IdType IdType { get; set; }
}

public class WorkerDetails {
    [Key] public int UserId { get; set; }
    public User? User { get; set; }

    public List<Job>? Jobs { get; set; }
    public List<PastJob>? PastJobs { get; set; }
}

public class Job {
    public int JobId { get; set; }

    public int UserId { get; set; }
    public WorkerDetails? WorkerDetails { get; set; }

    public required int WageRate { get; set; }
    public required JobCategory JobCategory { get; set; }
    public required JobExperience JobExperience { get; set; }
    public required JobGender JobGender { get; set; }
    public required JobType JobType { get; set; }
    [MaxLength(64)] public required string Locale { get; set; }

    public PermanentJob? PermanentJobDetails { get; set; }
}

public class PastJob {
    public required int PastJobId { get; set; }

    public required int UserId { get; set; }
    public WorkerDetails? WorkerDetails { get; set; }

    public required JobCategory JobCategory { get; set; }
    public required JobGender JobGender { get; set; }
    public required JobType JobType { get; set; }

    [MaxLength(15)] public required string EmployerPhoneNumber { get; set; }
    [MaxLength(64)] public required string Locale { get; set; }

    public required bool IsVerified { get; set; }
    public int? Rating { get; set; }
    [MaxLength(256)] public string? Comments { get; set; }
}

public class PermanentJob {
    [Key] public int JobId { get; set; }

    public int TrialPeriod { get; set; }
}