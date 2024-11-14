using FastEndpoints;
using sproj.Data;
using sproj.Services;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace sproj.Features.Verification;

public class SendSmsCodeEndpoint : Endpoint<EmptyRequest, EmptyResponse> {
    public required AppDbContext DbContext { get; set; }
    public required CodeVerifier CodeVerifier { get; set; }
    public required ISmsSender SmsSender { get; set; }

    public override void Configure() {
        Post("/verify/start-sms");
        Policy(p => p.RequireClaim("role", Role.Unregistered.ToString()));
    }

    public override async Task HandleAsync(EmptyRequest _, CancellationToken ct) {
        var phoneNumber = User.FindFirst("phone_number")!.Value;

        var code = await CodeVerifier.CreateCode(phoneNumber);
        SmsSender.SendMessage(phoneNumber, $"Your verification code is {code}");

        await SendOkAsync();
    }
}