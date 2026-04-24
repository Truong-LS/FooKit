using FluentValidation;
using MyProject.Application.DTOs.AuthDtos;

namespace MyProject.Application.Validators
{
    public class SetCredentialsRequestValidator : AbstractValidator<SetCredentialsRequest>
    {
        public SetCredentialsRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username must not be empty.")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
                .MaximumLength(50).WithMessage("Username must not exceed 50 characters.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password must not be empty.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Password confirmation does not match.");
        }
    }
}
