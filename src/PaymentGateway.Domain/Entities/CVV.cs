using PaymentGateway.Domain.Exceptions;
using PaymentGateway.Domain.Validation;

namespace PaymentGateway.Domain.Entities;

/// <summary>
/// Represents a card verification value (CVV), ensuring it is valid.
/// </summary>
public class Cvv
{
    /// <summary>
    /// The card's CVV (Card Verification Value).
    /// </summary>
    public string Value { get; }

    public Cvv(string cvv)
    {
        if (string.IsNullOrWhiteSpace(cvv))
        {
            throw new InvalidCvvException("CVV is required.");
        }

        if (!CardRegexes.CvvRegex().IsMatch(cvv))
        {
            throw new InvalidCvvException("CVV must be numeric and either 3 or 4 digits.");
        }

        Value = cvv;
    }
}