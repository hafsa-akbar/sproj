using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using sproj.Data;
using sproj.Services;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace sproj.Features.Verification;

public class StartCnicVerification : Endpoint<StartCnicVerification.Request, EmptyResponse> {
    public required AppDbContext DbContext { get; set; }
    public required JwtCreator JwtCreator { get; set; }
    public required ICnicVerificationService CnicVerificationService { get; set; }

    public override void Configure() {
        Post("/verify/cnic");
        Policy(p => p.RequireClaim("role", Role.Employer.ToString()));

        AllowFileUploads();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct) {
        var userId = int.Parse(User.FindFirst("user_id")!.Value);
        var user = await DbContext.Users.FirstAsync(u => u.UserId == userId);

        using var fileStream = req.Cnic.OpenReadStream();
        var cnic = await CnicVerificationService.VerifyCnicAsync(user, fileStream);

        if (cnic is null) ThrowError("cnic verification failed");

        user.CnicNumber = cnic;
        user.Role = Role.Worker;
        user.WorkerDetails = new WorkerDetails();
        await DbContext.SaveChangesAsync();

        await SendResultAsync(Results.Ok(new {
            Token = JwtCreator.CreateJwt(user)
        }));
    }

    public record Request(IFormFile Cnic);

    public class RequestValidator : Validator<Request> {
        public RequestValidator() {
            RuleFor(x => x.Cnic).NotNull();
        }
    }
}