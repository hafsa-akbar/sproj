using FastEndpoints;
using sproj.Data;
using sproj.Services;

namespace sproj.Features.Verification;

public class SendSmsCodeEndpoint : Endpoint<EmptyRequest, EmptyResponse> {
    public required AppDbContext DbContext { get; set; }
    public required CodeVerifier CodeVerifier { get; set; }
    public required ISmsSender SmsSender { get; set; }

    public override void Configure() {
        Post("/verify/start-sms");
        Policies("Unregistered");
    }

    public override async Task HandleAsync(EmptyRequest _, CancellationToken ct) {
        var phoneNumber = User.FindFirst("phone_number")!.Value;

        var code = await CodeVerifier.CreateCode(phoneNumber);
        SmsSender.SendMessage(phoneNumber, $"Your verification code is {code}");

        await SendOkAsync();
    }
}
