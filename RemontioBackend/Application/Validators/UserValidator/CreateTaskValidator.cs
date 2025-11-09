using Application.Objects.DTOs.TaskDTO;
using FluentValidation;
using System;

namespace Application.Validators.UserValidator
{
    public class CreateTaskValidator : AbstractValidator<CreateTaskDTO>
    {
        public CreateTaskValidator()
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

           
            RuleFor(x => x.StartAt)
                .Must((dto, start) => start == default || dto.CreateAt == default || start >= dto.CreateAt)
                .WithMessage("StartAt must be after creation time.");

            RuleFor(x => x.ClosedAt)
                .Must((dto, closed) => closed == default || dto.StartAt == default || closed >= dto.StartAt)
                .WithMessage("ClosedAt must be after StartAt.");
        }
    }
}
