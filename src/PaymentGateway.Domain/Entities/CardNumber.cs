using PaymentGateway.Domain.Exceptions;
using PaymentGateway.Domain.Validation;

namespace PaymentGateway.Domain.Entities;

/// <summary>
/// Represents a credit/debit card number, ensuring it is valid.
/// </summary>
public class CardNumber
{
    /// <summary>
    /// The full card number (in a real application, you'd avoid storing this in plain text).
    /// </summary>
    public string Value { get; }

    public CardNumber(string cardNumber)
    {
        string sanitized = cardNumber.Replace(" ", "");
        if (string.IsNullOrWhiteSpace(sanitized))
        {
            throw new InvalidCardNumberException("Card number is required.");
        }

        if (!CardRegexes.CardNumberRegex().IsMatch(sanitized))
        {
            throw new InvalidCardNumberException("Card number must be numeric and between 14 and 19 digits.");
        }

        Value = sanitized;
    }
}