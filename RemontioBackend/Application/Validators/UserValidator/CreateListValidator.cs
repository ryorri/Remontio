using Application.Objects.DTOs.ListDTO;
using FluentValidation;
using System;

namespace Application.Validators.UserValidator
{
    public class CreateListValidator : AbstractValidator<CreateListDTO>
    {
        public CreateListValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200);

            RuleFor(x => x.Description)
                .MaximumLength(1000);

            RuleFor(x => x.RoomId)
                .NotEqual(Guid.Empty).WithMessage("RoomId is required.");

            RuleFor(x => x.ProjectId)
                .NotEqual(Guid.Empty).WithMessage("ProjectId is required.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.")
                .MaximumLength(64);
        }
    }
}
