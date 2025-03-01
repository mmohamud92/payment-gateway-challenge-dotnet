using AutoMapper;

using MediatR;

using PaymentGateway.Application.Commands;
using PaymentGateway.Application.DTOs;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Enums;
using PaymentGateway.Domain.Exceptions;
using PaymentGateway.Domain.Interfaces;

namespace PaymentGateway.Application.Handlers;

public class ProcessPaymentHandler(IMapper mapper, IBankService bankService, IPaymentRepository paymentRepository)
    : IRequestHandler<ProcessPaymentCommand, PaymentResponseDto>
{
    public async Task<PaymentResponseDto> Handle(ProcessPaymentCommand command, CancellationToken cancellationToken)
    {
        PaymentRequestDto paymentRequestDto = command.PaymentRequest;

        Payment payment;
        try
        {
            payment = new Payment(
                command.MerchantId,
                paymentRequestDto.CardNumber,
                paymentRequestDto.ExpiryMonth,
                paymentRequestDto.ExpiryYear,
                paymentRequestDto.Amount,
                paymentRequestDto.Currency,
                paymentRequestDto.Cvv);
        }
        catch (Exception ex) when (ex is InvalidCardNumberException
                                       or InvalidExpiryDateException
                                       or InvalidCvvException
                                       or PaymentValidationException)
        {
            throw new PaymentValidationException(ex.Message);
        }

        BankPaymentRequestDto? bankRequest = mapper.Map<BankPaymentRequestDto>(payment);

        BankPaymentResponseDto bankResponse = await bankService.AuthorisePaymentAsync(bankRequest);

        PaymentStatus paymentStatus = bankResponse.Authorised ? PaymentStatus.Authorised : PaymentStatus.Declined;
        payment.UpdateStatus(paymentStatus);
        if (payment.Status == PaymentStatus.Authorised)
        {
            payment.SetAuthorisationCode(bankResponse.AuthorisationCode);
        }

        paymentRepository.AddPayment(payment);

        PaymentResponseDto? paymentResponseDto = mapper.Map<PaymentResponseDto>(payment);
        paymentResponseDto = paymentResponseDto with
        {
            LastFourCardDigits = payment.LastFourDigits,
            ExpiryMonth = payment.CardDetails.ExpiryDate.Month.ToString(),
            ExpiryYear = payment.CardDetails.ExpiryDate.Month.ToString(),
            Amount = payment.Denomination.Amount
        };

        return paymentResponseDto;
    }
}