using FluentValidation;
using TracklyApi.Dtos.Profile;

namespace TracklyApi.Validators;
public class ProfileDetailsValidator : AbstractValidator<ProfileDetailsDto>
{
    public ProfileDetailsValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("first name missing")
            .Length(2, 64).WithMessage("first name must be 2-64 symbols long");
        RuleFor(x => x.MiddleName)
            .Length(2, 64).When(x => x.MiddleName != null).WithMessage("middle name must be 2-64 symbols long");
        RuleFor(x => x.LastName)
            .Length(2, 64).When(x => x.LastName != null).WithMessage("last name must be 2-64 symbols long");
    }
}
