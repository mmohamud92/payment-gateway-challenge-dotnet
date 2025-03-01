using System.Security.Claims;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Application.Commands;
using PaymentGateway.Application.DTOs;
using PaymentGateway.Domain.Enums;
using PaymentGateway.Infrastructure;

namespace PaymentGateway.Api.Controllers;

[Route("api/payments")]
[ApiController]
public class PaymentsWriteController(IMediator mediator) : Controller
{
    /// <summary>
    /// Processes a payment request by accepting a <see cref="PaymentRequestDto"/> from the client.
    /// </summary>
    /// <param name="requestDto">
    /// The payment request details including card information, amount, currency, and CVV.
    /// The MerchantId is not supplied by the user and will be retrieved from the token.
    /// </param> 
    /// <returns>A 201 (Created) response containing the processed payment details in a <see cref="PaymentResponseDto"/>.</returns>\
    [Authorize(Policy = Constants.Policies.PaymentWritePolicy)]
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(201, Type = typeof(PaymentResponseDto))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequestDto requestDto)
    {
        Claim? merchantClaim = User.FindFirst(Constants.ClaimsExtension.MerchantId);
        if (merchantClaim == null || !Guid.TryParse(merchantClaim.Value, out Guid merchantId))
        {
            return BadRequest("Invalid or missing merchant ID in token.");
        }

        ProcessPaymentCommand command = new(requestDto, merchantId);
        PaymentResponseDto response = await mediator.Send(command);

        if (response.Status.Equals(PaymentStatus.Declined.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            return StatusCode(402, new { ReasonCode = "DECLINE" });
        }

        if (response.Status.Equals(PaymentStatus.Rejected.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest();
        }

        return CreatedAtAction(nameof(ProcessPayment), new { paymentId = response.Id }, response);
    }
}