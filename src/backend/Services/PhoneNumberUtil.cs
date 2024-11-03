using PhoneNumbers;

namespace sproj.Services;

public class PhoneNumberUtil {
    private readonly PhoneNumbers.PhoneNumberUtil _phoneNumberUtil;

    public PhoneNumberUtil() {
        _phoneNumberUtil = PhoneNumbers.PhoneNumberUtil.GetInstance();
    }

    public bool ValidatePhoneNumber(string phoneNumber) {
        try {
            return _phoneNumberUtil.IsValidNumber(_phoneNumberUtil.Parse(phoneNumber, "PK"));
        } catch (NumberParseException) {
            return false;
        }
    }

    public string NormalizePhoneNumber(string phoneNumber) {
        return _phoneNumberUtil.Format(_phoneNumberUtil.Parse(phoneNumber, "PK"), PhoneNumberFormat.E164);
    }
}