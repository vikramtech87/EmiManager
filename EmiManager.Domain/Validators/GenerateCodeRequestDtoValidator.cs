using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EmiManager.Domain.Dtos;

using FluentValidation;

namespace EmiManager.Domain.Validators; 

public class GenerateCodeRequestDtoValidator: AbstractValidator<GenerateCodeRequestDto> {
    public GenerateCodeRequestDtoValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(ValidationErrorConstants.Empty)
            .EmailAddress()
            .WithMessage(ValidationErrorConstants.Invalid);
    }
}
