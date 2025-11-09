using Application.Objects.DTOs.ContactsDTO;
using FluentValidation;
using System;

namespace Application.Validators.UserValidator
{
    public class CreateContactValidator : AbstractValidator<CreateContactDTO>
    {
        public CreateContactValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200);

            RuleFor(x => x.Description)
                .MaximumLength(1000);

            RuleFor(x => x.ContactDetails)
                .NotEmpty().WithMessage("ContactDetails is required.")
                .MaximumLength(500);

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.")
                .MaximumLength(64);
        }
    }
}
