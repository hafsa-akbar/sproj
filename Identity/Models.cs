using System.ComponentModel.DataAnnotations;

namespace sproj.Identity;

public class IdentityUser {
    public Guid Id { get; set; }
    
    [MaxLength(128)]
    public string UserName { get; set; }
    
    [MaxLength(128)]
    public string NormalizedUserName { get; set; }
}

public class IdentityRole {
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [MaxLength(128)]
    public string Name { get; set; }
}
