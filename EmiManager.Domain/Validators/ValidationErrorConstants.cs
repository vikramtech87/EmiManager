using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmiManager.Domain.Validators;

public static class ValidationErrorConstants {
    public static string Empty = "EMPTY";

    public static string Invalid = "INVALID";

    public static string TooShort = "TOO_SHORT";
    public static string TooLong = "TOO_LONG";

    public static string Duplicate = "DUPLICATE";

    public static string NonExistent = "NON_EXISTENT";
    public static string AlreadyVerified = "ALREADY_VERIFIED";

    public static string IncorrectAuth = "INCORRECT_AUTH";
    public static string UnverifiedEmail = "UNVERIFIED_EMAIL";
}
