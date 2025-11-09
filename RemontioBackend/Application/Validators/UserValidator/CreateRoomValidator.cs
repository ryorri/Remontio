using Application.Objects.DTOs.RoomDTO;
using Domain.Enums;
using FluentValidation;
using System;

namespace Application.Validators.UserValidator
{
    public class CreateRoomValidator : AbstractValidator<CreateRoomDTO>
    {
        public CreateRoomValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200);

            RuleFor(x => x.Description)
                .MaximumLength(1000);

            RuleFor(x => x.UserId)
                .NotEqual(Guid.Empty).WithMessage("UserId is required.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid status value.");
        }
    }
}
