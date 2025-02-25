namespace PaymentGateway.Domain.Entities;

/// <summary>
/// Represents the details of a payment card, including the card number,
/// expiry date, and CVV. Ensures that all values are valid.
/// </summary>
public class CardDetails
{
    /// <summary>
    /// The full card number, stored securely.
    /// </summary>
    public CardNumber CardNumber { get; }

    /// <summary>
    /// The expiry date of the card (month/year).
    /// </summary>
    public ExpiryDate ExpiryDate { get; }

    /// <summary>
    /// The card's CVV (Card Verification Value).
    /// </summary>
    public Cvv Cvv { get; }

    /// <summary>
    /// Initialises a new instance of the <see cref="CardDetails"/> class.
    /// Ensures all card details are valid before creation.
    /// </summary>
    public CardDetails(CardNumber cardNumber, ExpiryDate expiryDate, Cvv cvv)
    {
        CardNumber = cardNumber ?? throw new ArgumentNullException(nameof(cardNumber));
        ExpiryDate = expiryDate ?? throw new ArgumentNullException(nameof(expiryDate));
        Cvv = cvv ?? throw new ArgumentNullException(nameof(cvv));
    }
}