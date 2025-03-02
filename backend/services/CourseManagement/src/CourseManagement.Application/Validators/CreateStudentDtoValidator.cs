using CourseManagement.Application.DTOs.Students;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Application.Validators
{
    public class CreateStudentDtoValidator : AbstractValidator<CreateStudentDTO>
    {
        public CreateStudentDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full Name is required.")
                .MaximumLength(100).WithMessage("Full Name cannot exceed 100 characters.")
                .Matches("^[a-zA-Z0-9]*$").WithMessage("Name must be alphanumeric.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of Birth is required.")
                .Must(BeValidAge).WithMessage("Student must be at least 6 years old.");
        }

        private bool BeValidAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;

            return age >= 6;
        }
    }
}
