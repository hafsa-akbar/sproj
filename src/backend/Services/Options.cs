namespace sproj.Services;

public class JwtOptions {
    public const string SectionName = "JWT";

    public string? Key { get; set; } = null;
    public int Duration { get; set; } = 3600;
}