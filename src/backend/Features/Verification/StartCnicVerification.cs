using System.Security.Claims;
using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using sproj.Authentication;
using sproj.Data;
using sproj.Services;

namespace sproj.Features.Verification;

public class StartCnicVerification : Endpoint<StartCnicVerification.Request, EmptyResponse> {
    public required AppDbContext DbContext { get; set; }
    public required ISessionStore SessionStore { get; set; }
    public required ICnicVerificationService CnicVerificationService { get; set; }

    public override void Configure() {
        Post("/verify/cnic");
        Policies("Unregistered");

        AllowFileUploads();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct) {
        var userId = int.Parse(User.FindFirst("user_id")!.Value);
        var user = await DbContext.Users.Include(u => u.CnicVerification).FirstAsync(u => u.UserId == userId);

        if (user.Role == Role.Worker) {
            await SendUnauthorizedAsync();
            return;
        }

        using var fileStream = req.Cnic.OpenReadStream();
        using var ms = new MemoryStream();
        fileStream.CopyTo(ms);

        user.CnicVerification = new CnicVerification {
            IdImage = ms.ToArray(),
            IdType = IdType.Cnic
        };

        var saveImage = DbContext.SaveChangesAsync();

        var cnic = await CnicVerificationService.VerifyCnicAsync(user, fileStream);

        await saveImage;
        if (cnic is null)
            ThrowError("cnic verification failed");

        user.CnicNumber = cnic;
        user.Role = Role.Worker;
        user.WorkerDetails = new WorkerDetails();
        await DbContext.SaveChangesAsync();

        var sessionId = Guid.Parse(User.FindFirst("session_id")!.Value);
        var session = SessionStore.GetSession(sessionId)!;

        session.Claims.RemoveClaim(session.Claims.FindFirst("role"));
        session.Claims.AddClaim(new Claim("role", user.Role.ToString()));

        SessionStore.UpdateSession(sessionId, session);

        await SendResultAsync(Results.Ok());
    }

    public record Request(IFormFile Cnic);

    public class RequestValidator : Validator<Request> {
        public RequestValidator() {
            RuleFor(x => x.Cnic).NotNull();
        }
    }
}