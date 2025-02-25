using MediatR;

using PaymentGateway.Application.DTOs;

namespace PaymentGateway.Application.Queries;

public record GetPaymentByIdQuery(Guid PaymentId) : IRequest<PaymentResponseDto>;