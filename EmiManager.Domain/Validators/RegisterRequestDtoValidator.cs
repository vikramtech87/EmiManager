using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EmiManager.Domain.Dtos;

using FluentValidation;

namespace EmiManager.Domain.Validators;

public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto> {
    public RegisterRequestDtoValidator() {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(ValidationErrorConstants.Empty)
            .EmailAddress()
            .WithMessage(ValidationErrorConstants.Invalid);

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage(ValidationErrorConstants.Empty)
            .MinimumLength(8)
            .WithMessage(ValidationErrorConstants.TooShort)
            .MaximumLength(20)
            .WithMessage(ValidationErrorConstants.TooLong);

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ValidationErrorConstants.Empty)
            .MinimumLength(2)
            .WithMessage(ValidationErrorConstants.TooShort)
            .MaximumLength(128)
            .WithMessage(ValidationErrorConstants.TooLong);
    }
}
