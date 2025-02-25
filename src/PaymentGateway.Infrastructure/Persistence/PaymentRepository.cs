using System.Collections.Concurrent;

using Microsoft.Extensions.Caching.Memory;

using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Enums;
using PaymentGateway.Domain.Exceptions;
using PaymentGateway.Domain.Interfaces;

namespace PaymentGateway.Infrastructure.Persistence;

public class PaymentRepository : IPaymentRepository
{
    private readonly IMemoryCache _memoryCache;
    private readonly ConcurrentDictionary<Guid, Payment?> _payments;

    public PaymentRepository(IMemoryCache memoryCache)
    {
        _payments = new ConcurrentDictionary<Guid, Payment?>();
        _memoryCache = memoryCache;
        _payments = _memoryCache.GetOrCreate("Payments", entry => new ConcurrentDictionary<Guid, Payment>())!;
    }

    public Payment? GetPaymentById(Guid paymentId)
    {
        if (!_payments.TryGetValue(paymentId, out Payment? payment))
        {
            throw new PaymentNotFoundException($"No payment found with the specified id: {paymentId}.");
        }

        return payment;
    }

    public void AddPayment(Payment payment)
    {
        if (!_payments.TryAdd(payment.Id, payment))
        {
            throw new DuplicatePaymentException($"Payment already exists for payment id: {payment.Id}.");
        }

        _memoryCache.Set("Payments", _payments);
    }

    public void UpdatePaymentStatus(Guid paymentId, PaymentStatus newStatus)
    {
        if (!_payments.TryGetValue(paymentId, out Payment? existingPayment))
        {
            throw new PaymentNotFoundException($"No payment found with the specified ID: {paymentId}.");
        }

        existingPayment?.UpdateStatus(newStatus);

        _payments[paymentId] = existingPayment;
        _memoryCache.Set("Payments", _payments);
    }
}