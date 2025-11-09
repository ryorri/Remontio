using Application.Objects.DTOs.ProjectDTO;
using Domain.Enums;
using FluentValidation;

namespace Application.Validators.UserValidator
{
    public class CreateProjectValidator : AbstractValidator<CreateProjectDTO>
    {
        public CreateProjectValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200);

            RuleFor(x => x.Description)
                .MaximumLength(1000);

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.")
                .MaximumLength(64);

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid status value.");
        }
    }
}
