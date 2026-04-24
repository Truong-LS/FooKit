using FluentValidation;
using MyProject.Application.DTOs.AuthDtos;

namespace MyProject.Application.Validators;

public class LinkGoogleRequestValidator : AbstractValidator<LinkGoogleRequest>
{
    public LinkGoogleRequestValidator()
    {
        RuleFor(x => x.IdToken)
            .NotEmpty().WithMessage("Google ID Token is required.");
    }
}
