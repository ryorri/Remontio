using Application.Objects.DTOs.UserDTO;
using FluentValidation;

namespace Application.Validators.UserValidator
{
    public class CreateUserValidator : AbstractValidator<CreateUserDTO>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("UserName is required.")
                .MaximumLength(100);

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100);

            RuleFor(x => x.Surname)
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(200);


            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.")
                                                .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
                                                .Matches("[0-9]").WithMessage("Password must contain at least one number.")
                                                .Matches("[A-Z]").WithMessage("Password must contain at least capital letter.");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required.")
                .MaximumLength(50);
        }
    }
}
