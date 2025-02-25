using AutoMapper;

using MediatR;

using PaymentGateway.Application.DTOs;
using PaymentGateway.Application.Queries;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Interfaces;

namespace PaymentGateway.Application.Handlers;

public class GetPaymentByIdHandler(IPaymentRepository paymentRepository, IMapper mapper)
    : IRequestHandler<GetPaymentByIdQuery, PaymentResponseDto>
{
    public Task<PaymentResponseDto> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        Payment? payment = paymentRepository.GetPaymentById(request.PaymentId);
        PaymentResponseDto responseDto = mapper.Map<PaymentResponseDto>(payment);

        return Task.FromResult(responseDto);
    }
}