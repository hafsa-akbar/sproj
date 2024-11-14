#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace sproj.Services;

public class JwtOptions {
    public const string SectionName = "JWT";

    public string Key { get; set; }
    public int Duration { get; set; } = 3600;
}

public class TwilioOptions {
    public const string SectionName = "Twilio";

    public string AccountSid { get; set; }
    public string AuthToken { get; set; }
    public string PhoneNumber { get; set; }
}