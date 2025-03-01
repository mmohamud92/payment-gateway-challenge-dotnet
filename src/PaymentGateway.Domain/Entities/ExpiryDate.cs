using PaymentGateway.Domain.Exceptions;
using PaymentGateway.Domain.Validation;

namespace PaymentGateway.Domain.Entities;

/// <summary>
/// Represents an expiry date for a credit card.
/// </summary>
public class ExpiryDate
{
    /// <summary>
    /// The expiry month (1-12).
    /// </summary>
    public int Month { get; }
    /// <summary>
    /// The expiry year (four-digit).
    /// </summary>
    public int Year { get; }
    /// <summary>
    /// The expiry month formatted as a two-digit string.
    /// This is computed at construction so that single-digit months are zero-padded immediately.
    /// </summary>
    public string PaddedMonth => Month.ToString("D2");

    public ExpiryDate(string expiryMonthString, string expiryYearString)
    {
        if (!CardRegexes.ExpiryMonthRegex().IsMatch(expiryMonthString))
        {
            throw new InvalidExpiryDateException("The expiry month must be between 01 and 12.");
        }

        if (!CardRegexes.ExpiryYearRegex().IsMatch(expiryYearString))
        {
            throw new InvalidExpiryDateException("The expiry year must be a two or four digit number.");
        }

        int expiryMonth = int.Parse(expiryMonthString);
        int expiryYear = int.Parse(expiryYearString);

        if (expiryYear < 100)
        {
            int currentCentury = DateTime.UtcNow.Year / 100;
            int fullYear = currentCentury * 100 + expiryYear;

            expiryYear = fullYear;
        }

        if (expiryYear < DateTime.UtcNow.Year || expiryYear == DateTime.UtcNow.Year && expiryMonth < DateTime.UtcNow.Month)
        {
            throw new InvalidExpiryDateException("The expiry date must be in the future.");
        }

        Month = expiryMonth;
        Year = expiryYear;
    }
}