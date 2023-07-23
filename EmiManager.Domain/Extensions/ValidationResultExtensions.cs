using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation.Results;

namespace EmiManager.Domain.Extensions;

public static class ValidationResultExtensions {
    public static Dictionary<string, string> ToErrorDictionary(this ValidationResult result) {
        return result.Errors.Aggregate(
            new Dictionary<string, string>(),
            (Dictionary<string, string> errDict, FluentValidation.Results.ValidationFailure error) => {
                errDict.Add(error.PropertyName, error.ErrorMessage);
                return errDict;
            });
    }
}
