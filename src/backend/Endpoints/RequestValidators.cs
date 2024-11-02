using FluentValidation;

namespace sproj.Endpoints;

public class RegisterRequestValidator : AbstractValidator<UserEndpoints.RegisterRequest> {
    public RegisterRequestValidator() {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
        RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number is required.");
    }
}

public class LoginRequestValidator : AbstractValidator<UserEndpoints.LoginRequest> {
    public LoginRequestValidator() {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
    }
}
