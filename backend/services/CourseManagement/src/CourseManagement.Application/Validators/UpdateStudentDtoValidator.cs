using CourseManagement.Application.DTOs.Students;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Application.Validators
{
    public class UpdateStudentDtoValidator : AbstractValidator<UpdateStudentDTO>
    {
        public UpdateStudentDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full Name is required.")
                .MaximumLength(100).WithMessage("Full Name cannot exceed 100 characters.");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of Birth is required.")
                .Must(BeValidAge).WithMessage("Student must be at least 18 years old.");
        }

        private bool BeValidAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;

            return age >= 6;
        }
    }
}
