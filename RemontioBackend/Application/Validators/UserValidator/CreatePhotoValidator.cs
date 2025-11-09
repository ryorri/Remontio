using Application.Objects.DTOs.PhotoDTO;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace Application.Validators.UserValidator
{
    public class CreatePhotoValidator : AbstractValidator<CreatePhotoDTO>
    {
        private static readonly string[] AllowedContentTypes = new[]
        {
            "image/png","image/jpeg","image/jpg","image/gif","image/webp"
        };
        private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10MB

        public CreatePhotoValidator()
        {
            RuleFor(x => x.File)
                .NotNull().WithMessage("File is required.")
                .Must(f => f!.Length > 0).WithMessage("File cannot be empty.")
                .Must(f => f!.Length <= MaxFileSizeBytes).WithMessage("File exceeds maximum size of 10MB.")
                .Must(f => AllowedContentTypes.Contains(f!.ContentType ?? string.Empty))
                .WithMessage("Unsupported file type.");

            RuleFor(x => x.Description)
                .MaximumLength(500);

            RuleFor(x => x.StorageProvider)
                .MaximumLength(50);

            RuleFor(x => x.UserId)
                .MaximumLength(64);
        }
    }
}
