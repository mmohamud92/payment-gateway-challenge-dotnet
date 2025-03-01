namespace PaymentGateway.Domain.Exceptions;

/// <summary>
/// Exception thrown when a duplicate payment is attempted to be stored.
/// </summary>
public class DuplicatePaymentException(string message) : Exception(message);

/// <summary>
/// Exception thrown when a card number is invalid.
/// </summary>
public class InvalidCardNumberException(string message) : Exception(message);

/// <summary>
/// Exception thrown when a CVV is invalid.
/// </summary>
public class InvalidCvvException(string message) : Exception(message);

/// <summary>
/// Exception thrown when an expiry date is invalid.
/// </summary>
public class InvalidExpiryDateException(string message) : Exception(message);

/// <summary>
/// Exception thrown when the requested payment is not found
/// </summary>
public class PaymentNotFoundException(string message) : Exception(message);

/// <summary>
/// Exception there is an issue with the payment
/// </summary>
public class PaymentValidationException(string message) : Exception(message);