using PaymentGateway.Domain.Exceptions;

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

    public ExpiryDate(int expiryMonth, int expiryYear)
    {
        if (expiryMonth < 1 || expiryMonth > 12)
        {
            throw new InvalidExpiryDateException("The expiry month must be between 1 and 12.");
        }

        if (expiryYear < DateTime.UtcNow.Year || expiryYear == DateTime.UtcNow.Year && expiryMonth < DateTime.UtcNow.Month)
        {
            throw new InvalidExpiryDateException("The expiry date must be in the future.");
        }

        Month = expiryMonth;
        Year = expiryYear;
    }
}