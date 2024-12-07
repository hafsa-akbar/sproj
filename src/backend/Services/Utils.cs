using PhoneNumbers;

namespace sproj.Services;

public static class Utils {
    private static readonly PhoneNumberUtil PhoneNumberUtil = PhoneNumberUtil.GetInstance();

    public static bool ValidatePhoneNumber(string phoneNumber) {
        try {
            return PhoneNumberUtil.IsValidNumber(PhoneNumberUtil.Parse(phoneNumber, "PK"));
        } catch (NumberParseException) {
            return false;
        }
    }

    public static string NormalizePhoneNumber(string phoneNumber) {
        return PhoneNumberUtil.Format(PhoneNumberUtil.Parse(phoneNumber, "PK"), PhoneNumberFormat.E164);
    }
}