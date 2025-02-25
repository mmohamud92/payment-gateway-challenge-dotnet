using AutoMapper;

using PaymentGateway.Application.DTOs;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Application.MappingProfiles;

public class PaymentMappingProfile : Profile
{
    public PaymentMappingProfile()
    {
        CreateMap<Payment, BankPaymentRequestDto>()
            .ConstructUsing(src => new BankPaymentRequestDto(
                src.CardDetails.CardNumber.Value,
                $"{src.CardDetails.ExpiryDate.Month}/{src.CardDetails.ExpiryDate.Year}",
                src.Denomination.Currency.ToString(),
                src.Denomination.Amount,
                src.CardDetails.Cvv.Value
            ));

        CreateMap<Payment, PaymentResponseDto>()
            .ConstructUsing(src => new PaymentResponseDto(
                src.Id,
                src.Status.ToString(),
                src.LastFourDigits,
                src.CardDetails.ExpiryDate.Month,
                src.CardDetails.ExpiryDate.Year,
                src.Denomination.Currency.ToString(),
                src.Denomination.Amount
            ));
    }
}