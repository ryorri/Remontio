using Application.Objects.DTOs.BudgetItemDTO;
using FluentValidation;

namespace Application.Validators.UserValidator
{
    public class CreateBudgetItemValidator : AbstractValidator<CreateBudgetItemDTO>
    {
        public CreateBudgetItemValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200);

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be non-negative.");

            RuleFor(x => x.Total)
                .GreaterThanOrEqualTo(0).WithMessage("Total must be non-negative.");

            RuleFor(x => x.EstimatetPrice)
                .GreaterThanOrEqualTo(0).WithMessage("EstimatedPrice must be non-negative.")
                .GreaterThanOrEqualTo(x => x.Total).WithMessage("EstimatedPrice must be at least Total.");
        }
    }
}
