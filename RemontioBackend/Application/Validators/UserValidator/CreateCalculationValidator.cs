using Application.Objects.DTOs.CalculationsDTO;
using FluentValidation;

namespace Application.Validators.UserValidator
{
    public class CreateCalculationValidator : AbstractValidator<CreateCalculationDTO>
    {
        public CreateCalculationValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200);

            RuleFor(x => x.Value)
                .GreaterThanOrEqualTo(0).WithMessage("Value must be non-negative.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid calculation type.");
        }
    }
}
