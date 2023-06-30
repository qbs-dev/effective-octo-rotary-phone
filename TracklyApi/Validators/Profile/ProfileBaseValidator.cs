using FluentValidation;
using TracklyApi.Dtos.Profile;

namespace TracklyApi.Validators;
public class ProfileBaseValidator : AbstractValidator<ProfileBaseDto>
{
    public ProfileBaseValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("first name missing")
            .Length(2, 64).WithMessage("first name must be 2-64 symbols long");
        RuleFor(x => x.MiddleName)
            .Length(2, 64).When(x => x.MiddleName.Length > 0).WithMessage("middle name must be 2-64 symbols long");
        RuleFor(x => x.LastName)
            .Length(2, 64).When(x => x.LastName.Length > 0).WithMessage("last name must be 2-64 symbols long");
    }
}
