using MediatR;

using PaymentGateway.Application.DTOs;

namespace PaymentGateway.Application.Commands;

public record ProcessPaymentCommand(PaymentRequestDto PaymentRequest, Guid MerchantId) : IRequest<PaymentResponseDto>;