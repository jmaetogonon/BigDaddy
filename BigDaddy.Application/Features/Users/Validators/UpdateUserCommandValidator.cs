using BigDaddy.Application.Features.Users.Commands.UpdateUser;
using FluentValidation;

namespace BigDaddy.Application.Features.Users.Validators;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Username)
            .NotEmpty().MinimumLength(3).MaximumLength(100)
            .Matches(@"^[a-zA-Z0-9._-]+$")
            .WithMessage("Username may only contain letters, numbers, dots, hyphens, and underscores.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.MobileNumber)
            .Matches(@"^\+?[0-9\s\-\(\)]{7,20}$")
            .When(x => !string.IsNullOrWhiteSpace(x.MobileNumber))
            .WithMessage("Mobile number format is invalid.");
    }
}