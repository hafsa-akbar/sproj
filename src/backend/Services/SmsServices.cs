using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace sproj.Services;

public interface ISmsSender {
    public void SendCode(string to, string code);
}

public class SmsSender : ISmsSender {
    private readonly TwilioOptions _twilioOptions;

    public SmsSender(TwilioOptions twilioOptions) {
        _twilioOptions = twilioOptions;
        TwilioClient.Init(_twilioOptions.AccountSid, _twilioOptions.AuthToken);
    }

    public void SendCode(string to, string code) {
        try {
            var message = MessageResource.Create(
                from: new PhoneNumber(_twilioOptions.PhoneNumber),
                to: new PhoneNumber(to),
                body: $"Your verification code is: {code}"
            );

            Console.WriteLine($"Message sent to {to}: Status {message.Status}");
        } catch (Exception ex) {
            Console.WriteLine($"Error sending SMS to {to}: {ex.Message}");
            throw;
        }
    }
}

public class DummySmsSender : ISmsSender {
    private readonly ILogger<DummySmsSender> _logger;

    public DummySmsSender(ILogger<DummySmsSender> logger) {
        _logger = logger;
    }

    public void SendCode(string to, string code) {
        _logger.LogInformation("Sending code {code} to {to}", code, to);
    }
}