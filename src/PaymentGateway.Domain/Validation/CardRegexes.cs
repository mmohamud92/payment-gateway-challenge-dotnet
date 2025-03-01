using System.Text.RegularExpressions;

namespace PaymentGateway.Domain.Validation;

public static partial class CardRegexes
{
    [GeneratedRegex(@"^\d{14,19}$")]
    public static partial Regex CardNumberRegex();

    [GeneratedRegex(@"^\d{3,4}$")]
    public static partial Regex CvvRegex();

    [GeneratedRegex(@"^(0?[1-9]|1[0-2])$")]
    public static partial Regex ExpiryMonthRegex();

    [GeneratedRegex(@"^(\d{2}|\d{4})$")]
    public static partial Regex ExpiryYearRegex();
}