using FluentValidation;
using TracklyApi.Dtos.Profile;

namespace TracklyApi.Validators;
public class RegisterValidator : AbstractValidator<RegisterDto>
{
    public RegisterValidator(ProfileDetailsValidator profileValidator)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("email address missing")
            .EmailAddress().WithMessage("incorrect email adress")
            .MaximumLength(128).WithMessage("email address can't be longer than 128 symbols");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("password missing")
            .Length(64).WithMessage("incorrect password hash length");
    }
}
