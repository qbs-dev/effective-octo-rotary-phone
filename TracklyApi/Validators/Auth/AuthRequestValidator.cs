using FluentValidation;
using TracklyApi.Dtos.Auth;

namespace TracklyApi.Validators;
public class AuthRequestValidator : AbstractValidator<AuthRequestDto>
{
    public AuthRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("email address missing")
            .EmailAddress().WithMessage("incorrect email adress")
            .Length(3, 128).WithMessage("email address must be 3-128 symbols long");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("password missing")
            .Length(64).WithMessage("incorrect password hash length");
        RuleFor(x => x.Fingerprint)
            .NotEmpty().WithMessage("browser fingerprint missing")
            .MaximumLength(128).WithMessage("browser fingerprint can't be longer than 128 symbols");
    }
}
