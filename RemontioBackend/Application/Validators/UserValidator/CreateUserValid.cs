using Application.Objects.DTOs.UserDTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.UserValidator
{
    public class CreateUserValid : AbstractValidator<CreateUserDTO>
    {
        public CreateUserValid() 
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required.");

            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");

            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.")
                                            .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.")
                                                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                                                .Matches("[0-9]").WithMessage("Password must contain at least one number.")
                                                .Matches("[A-Z]").WithMessage("Password must contain at least capital letter.");                                               
        }
    }
}
