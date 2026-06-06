using FluentValidation;
using FooKit.Application.DTOs.AuthDtos;

namespace FooKit.Application.Validators;

public class LinkGoogleRequestValidator : AbstractValidator<LinkGoogleRequest>
{
    public LinkGoogleRequestValidator()
    {
        RuleFor(x => x.IdToken)
            .NotEmpty().WithMessage("Google ID Token is required.");
    }
}
