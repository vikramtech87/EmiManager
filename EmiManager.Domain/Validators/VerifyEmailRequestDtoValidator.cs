using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EmiManager.Domain.Dtos;

using FluentValidation;

namespace EmiManager.Domain.Validators;

public class VerifyEmailRequestDtoValidator : AbstractValidator<VerifyEmailRequestDto> {
    public VerifyEmailRequestDtoValidator() {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(ValidationErrorConstants.Empty)
            .EmailAddress()
            .WithMessage(ValidationErrorConstants.Invalid);

        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage(ValidationErrorConstants.Empty);
    }
}
