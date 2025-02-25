using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Enums;

namespace PaymentGateway.Domain.Interfaces;

public interface IPaymentRepository
{
    Payment? GetPaymentById(Guid paymentId);
    void AddPayment(Payment payment);
    void UpdatePaymentStatus(Guid paymentId, PaymentStatus newStatus);
}