using Application.Objects.DTOs.AlertsDTO;
using FluentValidation;
using System;

namespace Application.Validators.UserValidator
{
    public class CreateAlertValidator : AbstractValidator<CreateAlertDTO>
    {
        public CreateAlertValidator()
        {
            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message is required.")
                .MaximumLength(500).WithMessage("Message must be at most 500 characters.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.")
                .MaximumLength(64);

            RuleFor(x => x.StartDate)
                .NotEqual(default(DateTime)).WithMessage("StartDate is required.");

            RuleFor(x => x.EndDate)
                .NotEqual(default(DateTime)).WithMessage("EndDate is required.")
                .GreaterThan(x => x.StartDate).WithMessage("EndDate must be greater than StartDate.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.")
                .MaximumLength(50);

            RuleFor(x => x.Priority)
                .NotEmpty().WithMessage("Priority is required.")
                .MaximumLength(50);
        }
    }
}
