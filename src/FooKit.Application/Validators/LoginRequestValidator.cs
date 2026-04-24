using FluentValidation;
using MyProject.Application.DTOs.AuthDtos;

namespace MyProject.Application.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("The username cannot be left blank.")
                .MinimumLength(3).WithMessage("Usernames must be at least 3 characters long.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("The password cannot be left blank.")
                .MinimumLength(6).WithMessage("Passwords must be at least 6 characters long.");
        }
    }
}
