using System.Net;
using FluentValidation;
using TracklyApi.Dtos.Url;

namespace TracklyApi.Validators;
public class RedirectRequestValidator : AbstractValidator<RedirectRequestDto>
{
    public RedirectRequestValidator()
    {
        RuleFor(x => x.Path)
            .Length(3, 64).WithMessage("path must be 3-64 symbols long")
            .Must(x => BeAValidURLPath(x)).WithMessage("incorrect path format");
        RuleFor(x => x.IpAddressString)
            .MaximumLength(50).WithMessage("ip address can't be longer than 50 symbols")
            .Must(x => IPAddress.TryParse(x, out _)).WithMessage("incorrect ip address format");
    }

    public bool BeAValidURLPath(string urlPath)
    {
        return Uri.TryCreate(urlPath, UriKind.Relative, out _);
    }
}
