using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using sproj.Services;

// ReSharper disable CollectionNeverUpdated.Global

namespace sproj.Data;

public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Job> Jobs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        base.OnConfiguring(optionsBuilder.UseSnakeCaseNamingConvention());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        // TODO: Use generic function to clean this
        modelBuilder.Entity<Locale>(entity => {
            entity.HasData(
                Utils.GetEnumNames<Locales>().Select(e => new Locale {
                    // TODO: Use strong types
                    LocaleId = e.Item1,
                    LocaleName = e.Item2
                })
            );
        });

        modelBuilder.Entity<JobCategory>(entity => {
            entity.HasData(
                Utils.GetEnumNames<JobCategories>().Select(e => new JobCategory {
                    JobCategoryId = e.Item1,
                    JobCategoryDescription = e.Item2
                })
            );
        });

        modelBuilder.Entity<JobType>(entity => {
            entity.HasData(
                Utils.GetEnumNames<JobTypes>().Select(e => new JobType {
                    JobTypeId = e.Item1,
                    JobTypeDescription = e.Item2
                })
            );
        });

        modelBuilder.Entity<JobExperience>(entity => {
            entity.HasData(
                Utils.GetEnumNames<JobExperiences>().Select(e => new JobExperience {
                    JobExperienceId = e.Item1,
                    JobExperienceDescription = e.Item2
                })
            );
        });

        modelBuilder.Entity<Role>(entity => {
            entity.HasData(
                Utils.GetEnumNames<Roles>().Select(e => new Role {
                    RoleId = e.Item1,
                    RoleDescription = e.Item2
                })
            );
        });

        modelBuilder.Entity<UserPreferences>()
            .HasMany(up => up.JobCategories)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "job_category_prefs",
                j => j.HasOne<JobCategory>().WithMany().HasForeignKey("job_category_id"),
                j => j.HasOne<UserPreferences>().WithMany().HasForeignKey("user_id")
            ).HasKey("user_id", "job_category_id");

        modelBuilder.Entity<UserPreferences>()
            .HasMany(up => up.JobTypes)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "job_type_prefs",
                j => j.HasOne<JobType>().WithMany().HasForeignKey("job_type_id"),
                j => j.HasOne<UserPreferences>().WithMany().HasForeignKey("user_id")
            ).HasKey("user_id", "job_type_id");

        modelBuilder.Entity<UserPreferences>()
            .HasMany(up => up.JobExperiences)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "job_experience_prefs",
                j => j.HasOne<JobExperience>().WithMany().HasForeignKey("job_experience_id"),
                j => j.HasOne<UserPreferences>().WithMany().HasForeignKey("user_id")
            ).HasKey("user_id", "job_experience_id");

        modelBuilder.Entity<Job>().HasOne(j => j.WorkerDetails).WithOne().HasForeignKey<Job>(j => j.UserId);
    }
}

public class User {
    public int UserId { get; set; }

    [MaxLength(15)] public required string PhoneNumber { get; set; }
    [MaxLength(128)] public required string Password { get; set; }

    public required Roles RoleId { get; set; }
    public Role? Role { get; set; }

    [MaxLength(128)] public required string FullName { get; set; }
    [MaxLength(512)] public required string Address { get; set; }
    public required DateOnly Birthdate { get; set; }
    public required UserGenders Gender { get; set; }
    [MaxLength(32)] public string? CnicNumber { get; set; }
    [MaxLength(32)] public string? DrivingLicense { get; set; }

    public SmsVerification? SmsVerifications { get; set; }
    public CnicVerification? CnicVerifications { get; set; }
    public UserPreferences? UserPreferences { get; set; }
    public WorkerDetails? WorkerDetails { get; set; }
}

public class WorkerDetails {
    [Key] public int UserId { get; set; }

    public User? User { get; set; }
}

public class SmsVerification {
    [Key] public int UserId { get; set; }
    [MaxLength(6)] public required string VerificationCode { get; set; }
    public required DateTime ExpiresAt { get; set; }

    public User? User { get; set; }
}

public class CnicVerification {
    [Key] public int UserId { get; set; }
    public required byte[] CnicImage { get; set; }

    public User? User { get; set; }
}

public class UserPreferences {
    [Key] public int UserId { get; set; }

    public Locales? JobLocale { get; set; }
    public ICollection<JobCategory>? JobCategories { get; set; }
    public ICollection<JobType>? JobTypes { get; set; }
    public ICollection<JobExperience>? JobExperiences { get; set; }
}

public class Job {
    public int JobId { get; set; }

    // public required bool IsCoupleJob { get; set; }
    public required int WageRate { get; set; }

    public required int UserId { get; set; }
    public WorkerDetails? WorkerDetails { get; set; }

    public required JobCategories JobCategoryId { get; set; }
    public JobCategory? JobCategory { get; set; }

    public required JobTypes JobTypeId { get; set; }
    public JobType? JobType { get; set; }

    public required JobExperiences JobExperienceId { get; set; }
    public JobExperience? JobExperience { get; set; }

    public required Locales LocaleId { get; set; }
    public Locale? Locale { get; set; }

    public PermanentJob? PermanentJobDetails { get; set; }
}

public class PermanentJob {
    [Key] public int JobId { get; set; }

    public int TrialPeriod { get; set; }
}
