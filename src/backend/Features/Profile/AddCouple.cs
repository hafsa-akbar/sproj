using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using sproj.Data;
using sproj.Services;

namespace sproj.Features.Profile;

// TODO: Add by phone number instead
// TODO: Approval system
public class AddCouple : Endpoint<AddCouple.Request, AddCouple.Response> {
    public required AppDbContext DbContext { get; set; }

    public override void Configure() {
        Post("/profile/add-couple");
        Policies("Worker");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct) {
        var userId = int.Parse(User.FindFirst("user_id")!.Value);
        var user = await DbContext.Users.FirstAsync(u => u.UserId == userId);

        var normalizedPhoneNumber = Utils.NormalizePhoneNumber(req.PhoneNumber);
        var couple = await DbContext.Users.FirstOrDefaultAsync(u => u.PhoneNumber == normalizedPhoneNumber);

        if (couple is null)
            ThrowError("user does not exist");

        if (couple.Gender == user.Gender)
            ThrowError("gender must be different");
        
        if (couple.Couple is not null)
            ThrowError("user is already in a relationship");
        
        if (couple.Role != Role.Worker)
            ThrowError("user is not a worker");

        user.Couple = couple;
        couple.Couple = user;
        await DbContext.SaveChangesAsync();

        await SendOkAsync(new Response(couple.FullName), ct);
    }

    public record Request(string PhoneNumber);
    public record struct Response(string CoupleName);
    public class RequestValidator : Validator<Request> {
        public RequestValidator() {
            RuleFor(x => x.PhoneNumber).NotEmpty().Must(Utils.ValidatePhoneNumber);
        }
    }
}