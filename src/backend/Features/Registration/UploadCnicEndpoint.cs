using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using sproj.Data;
using sproj.Services;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace sproj.Features.Registration;

public class UploadCnicEndpoint : Endpoint<UploadCnicEndpoint.Request, EmptyResponse> {
    public required AppDbContext DbContext { get; set; }
    public required ICnicVerificationService CnicVerificationService { get; set; }

    public override void Configure() {
        Post("/users/cnic-upload");
        AllowAnonymous();
        Policy(p => p.RequireClaim("role", ((int)Data.Roles.Employer).ToString()));

        AllowFileUploads();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct) {
        var userId = int.Parse(User.FindFirst("user_id")!.Value);
        var user = await DbContext.Users.FirstAsync(u => u.UserId == userId);

        using var fileStream = req.Cnic.OpenReadStream();
        var cnic = await CnicVerificationService.VerifyCnicAsync(user, fileStream);

        if (cnic is null) {
            await SendResultAsync(Results.BadRequest("CNIC verification failed"));
            return;
        }

        user.CnicNumber = cnic;
        user.RoleId = Data.Roles.Worker;

        await DbContext.SaveChangesAsync();
        await SendResultAsync(Results.Ok("CNIC verified successfully"));
    }

    public record Request(IFormFile Cnic);

    public class RequestValidator : Validator<Request> {
        public RequestValidator() {
            RuleFor(x => x.Cnic).NotNull();
        }
    }
}
