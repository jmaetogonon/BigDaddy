using BigDaddy.Application.Features.Users.Commands.ChangePassword;
using FluentValidation;

namespace BigDaddy.Application.Features.Users.Validators;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(x => x.NewPassword)
            .NotEmpty().MinimumLength(8)
            .Matches(@"[A-Z]").WithMessage("Must contain an uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Must contain a lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Must contain a digit.")
            .Matches(@"[^a-zA-Z0-9]").WithMessage("Must contain a special character.")
            .NotEqual(x => x.CurrentPassword).WithMessage("New password must differ from current.");
        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.NewPassword).WithMessage("Passwords do not match.");
    }
}