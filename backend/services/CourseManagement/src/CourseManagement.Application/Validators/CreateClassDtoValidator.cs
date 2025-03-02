using CourseManagement.Application.DTOs.Classes;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Application.Validators
{
    public class CreateClassDtoValidator : AbstractValidator<CreateClassDTO>
    {
        public CreateClassDtoValidator()
        {
            RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(50).WithMessage("Name cannot exceed 50 characters.")
            .Matches("^[a-zA-Z0-9]*$").WithMessage("Name must be alphanumeric.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(200).WithMessage("Description cannot exceed 200 characters.");
        }
    }
}
