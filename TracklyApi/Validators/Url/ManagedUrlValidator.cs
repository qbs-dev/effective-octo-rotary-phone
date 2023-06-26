using FluentValidation;
using TracklyApi.Dtos.Url;

namespace TracklyApi.Validators;
public class ManagedUrlValidator : AbstractValidator<UrlDto>
{
    public ManagedUrlValidator()
    {
        RuleFor(x => x.NewPath)
            .Length(3, 64).WithMessage("path must be 3-64 symbols long")
            .Must(x => BeAValidURLPath(x)).WithMessage("incorrect path format");
        RuleFor(x => x.TargetUrl)
            .Length(6, 128).WithMessage("target url must be 6-128 symbols long")
            .Must(x => BeAValidURL(x)).WithMessage("incorrect target url format");
        RuleFor(x => x.Description)
            .MaximumLength(256).WithMessage("description can't be longer than 256 symbols");
    }

    public bool BeAValidURLPath(string urlPath)
    {
        return Uri.TryCreate(urlPath, UriKind.Relative, out _);
    }

    public bool BeAValidURL(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}
