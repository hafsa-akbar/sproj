using FastEndpoints;
using sproj.Data;
using sproj.Services;

// ReSharper disable NotAccessedPositionalProperty.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace sproj.Features.Registration;

public class SendSmsCodeEndpoint : Endpoint<EmptyRequest, EmptyResponse> {
    public required AppDbContext DbContext { get; set; }
    public required CodeVerifier CodeVerifier { get; set; }
    public required ISmsSender SmsSender { get; set; }

    public override void Configure() {
        Post("/users/send-sms-code");
        Policy(p => p.RequireClaim("isPhoneVerified", "false"));
    }

    // TODO: Add rate limiting
    public override async Task HandleAsync(EmptyRequest _, CancellationToken ct) {
        var phoneNumber = User.FindFirst("phoneNumber")!.Value;

        var code = CodeVerifier.CreateCode(phoneNumber);
        SmsSender.SendCode(phoneNumber, code);

        await SendResultAsync(Results.Ok(new {
            Status = "success"
        }));
    }
}