using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace sproj.Services;

public interface ISmsSender {
    public void SendMessage(string to, string text);
}

public class SmsSender : ISmsSender {
    private readonly ILogger<SmsSender> _logger;
    private readonly TwilioOptions _twilioOptions;

    public SmsSender(TwilioOptions twilioOptions, ILogger<SmsSender> logger) {
        TwilioClient.Init(twilioOptions.AccountSid, twilioOptions.AuthToken);
        _twilioOptions = twilioOptions;
        _logger = logger;
    }

    public void SendMessage(string to, string text) {
        try {
            var messageResource = MessageResource.Create(
                from: new PhoneNumber(_twilioOptions.PhoneNumber),
                to: new PhoneNumber(to),
                body: $"{text}"
            );

            _logger.LogInformation("SMS sent to {recipient}. Status: {status}", to, messageResource.Status);
        } catch (Exception ex) {
            _logger.LogError("Error sending sms to {recipient}: {exception}", to, ex.Message);
        }
    }
}

public class DummySmsSender : ISmsSender {
    private readonly ILogger<DummySmsSender> _logger;

    public DummySmsSender(ILogger<DummySmsSender> logger) {
        _logger = logger;
    }

    public void SendMessage(string to, string text) {
        _logger.LogInformation("Sending a dummy text to {recipient}. Contents: {text}", to, text);
    }
}