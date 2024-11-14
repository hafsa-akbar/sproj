using PhoneNumbers;

namespace sproj.Services;

public static class Utils {
    private static readonly PhoneNumberUtil PhoneNumberUtil = PhoneNumberUtil.GetInstance();

    public static List<(TEnum, string)> GetEnumNames<TEnum>() where TEnum : Enum {
        var enumNames = new List<(TEnum, string)>();

        foreach (var value in Enum.GetValues(typeof(TEnum))) {
            var name = Enum.GetName(typeof(TEnum), value)!;

            var snakeCaseName = string.Concat(name.Select((ch, index) =>
                index > 0 && char.IsUpper(ch)
                    ? "_" + char.ToLowerInvariant(ch)
                    : char.ToLowerInvariant(ch).ToString()));

            enumNames.Add(((TEnum)value, snakeCaseName));
        }

        return enumNames;
    }

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
