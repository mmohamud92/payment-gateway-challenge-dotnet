using System;

using PaymentGateway.Domain.Enums;

namespace PaymentGateway.Domain.Entities;

/// <summary>
/// Represents a payment and encapsulates all related value objects and entities.
/// </summary>
public class Payment
{
    public Guid Id { get; }
    public CardDetails CardDetails { get; }
    public Denomination Denomination { get; }
    public Guid MerchantId { get; }
    public DateTime Timestamp { get; }
    public string? AuthorisationCode { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string LastFourDigits { get; private set; }

    /// <summary>
    /// Constructs a new Payment by creating all internal entities.
    /// </summary>
    /// <param name="merchantId">Merchant identifier (must not be empty).</param>
    /// <param name="cardNumber">The full card number (will be validated).</param>
    /// <param name="expiryMonth">The card's expiry month.</param>
    /// <param name="expiryYear">The card's expiry year.</param>
    /// <param name="amount">The payment amount (non-negative, expressed in minor units if needed).</param>
    /// <param name="currency">The currency code (e.g., "USD").</param>
    /// <param name="cvv">The card's CVV (validated for proper format).</param>
    /// <exception cref="ArgumentException">Thrown when inputs are invalid.</exception>
    public Payment(Guid merchantId, string cardNumber, int expiryMonth, int expiryYear, int amount, string currency,
        string cvv)
    {
        if (merchantId == Guid.Empty)
        {
            throw new ArgumentException("Merchant ID is required.", nameof(merchantId));
        }

        Denomination = new Denomination(amount, currency);

        CardNumber cardNumberObj = new(cardNumber);
        ExpiryDate expiryDateObj = new(expiryMonth, expiryYear);
        Cvv cvvObj = new(cvv);

        Id = Guid.NewGuid();
        CardDetails = new CardDetails(cardNumberObj, expiryDateObj, cvvObj);
        MerchantId = merchantId;
        Timestamp = DateTime.UtcNow;
        Status = PaymentStatus.Pending;

        LastFourDigits = cardNumberObj.Value.Substring(cardNumberObj.Value.Length - 4);
    }

    /// <summary>
    /// Updates the payment status following allowed transitions.
    /// </summary>
    /// <param name="newStatus">The new status to transition to.</param>
    /// <exception cref="InvalidOperationException">Thrown if the transition is not allowed.</exception>
    public void UpdateStatus(PaymentStatus newStatus)
    {
        switch (newStatus)
        {
            case var _ when newStatus == Status:
                throw new InvalidOperationException("Payment status is already set to this value.");

            case PaymentStatus.Authorised when Status != PaymentStatus.Pending:
            case PaymentStatus.Declined when Status != PaymentStatus.Pending:
                throw new InvalidOperationException("Only payments in the Pending state can be updated.");

            case PaymentStatus.Authorised:
            case PaymentStatus.Declined:
                Status = newStatus;
                break;

            default:
                throw new InvalidOperationException(
                    "Invalid status transition. Payments can only transition from Pending to Authorised or Declined.");
        }
    }

    /// <summary>
    /// Sets the authorisation code if the payment has been authorised.
    /// </summary>
    /// <param name="authorisationCode">The authorisation code provided by the bank.</param>
    public void SetAuthorisationCode(string authorisationCode)
    {
        if (Status == PaymentStatus.Declined)
        {
            throw new InvalidOperationException("Authorization code can only be set when the payment is authorised.");
        }

        AuthorisationCode = authorisationCode;
    }
}