using Application.Objects.DTOs.BudgetDTO;
using FluentValidation;
using System;

namespace Application.Validators.UserValidator
{
    public class CreateBudgetValidator : AbstractValidator<CreateBudgetDTO>
    {
        public CreateBudgetValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200);

            RuleFor(x => x.Description)
                .MaximumLength(1000);

            RuleFor(x => x.Total)
                .GreaterThanOrEqualTo(0).WithMessage("Total must be non-negative.");

            RuleFor(x => x.Spent)
                .GreaterThanOrEqualTo(0).WithMessage("Spent must be non-negative.")
                .LessThanOrEqualTo(x => x.Total).WithMessage("Spent cannot exceed Total.");

            RuleFor(x => x.EstimatedPrice)
                .GreaterThanOrEqualTo(0).WithMessage("EstimatedPrice must be non-negative.")
                .GreaterThanOrEqualTo(x => x.Spent).WithMessage("EstimatedPrice must be at least Spent.");

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
