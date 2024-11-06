using System.ComponentModel.DataAnnotations;

namespace sproj.Data;

public class Locale {
    public Locales LocaleId { get; set; }
    [MaxLength(32)] public required string LocaleName { get; set; }
}

public class JobCategory {
    public JobCategories JobCategoryId { get; set; }
    [MaxLength(32)] public required string JobCategoryDescription { get; set; }
}

public class JobType {
    public JobTypes JobTypeId { get; set; }
    [MaxLength(32)] public required string JobTypeDescription { get; set; }
}

public class JobExperience {
    public JobExperiences JobExperienceId { get; set; }
    [MaxLength(32)] public required string JobExperienceDescription { get; set; }
}

public class Role {
    public Roles RoleId { get; set; }
    [MaxLength(32)] public required string RoleDescription { get; set; }
}

public enum Locales
{
    Lahore = 1,
    Islamabad = 2
}

public enum JobCategories
{
    Cooking = 1,
    Cleaning = 2,
    Driving = 3,
    Laundry = 4,
    Gardening = 5,
    Babysitting = 6,
    PetCare = 7,
    SecurityGuard = 8
}

public enum JobTypes
{
    OneShot = 1,
    PermanentHire = 2
}

public enum JobExperiences
{
    Beginner = 1,
    Intermediate = 2,
    Expert = 3
}

public enum Roles
{
    Unregistered = 1,
    Employer = 2,
    Worker = 3
}
