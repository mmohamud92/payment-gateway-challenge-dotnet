using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Application.DTOs;
using PaymentGateway.Application.Queries;
using PaymentGateway.Infrastructure;
using PaymentGateway.Infrastructure.Authentication;

namespace PaymentGateway.Api.Controllers;

[Route("api/payments")]
[ApiController]
public class PaymentsReadController(IMediator mediator) : Controller
{
    /// <summary>
    /// Retrieves details of a previously processed payment using its unique identifier.
    /// </summary>
    /// <param name="paymentId">The unique identifier of the payment.</param>
    /// <returns>
    /// Returns a 200 (OK) response with a <see cref="PaymentResponseDto"/> if found, or a 404 (Not Found) if the payment does not exist.
    /// </returns>
    [Authorize(Policy = Constants.Policies.PaymentReadPolicy)]
    [HttpGet("{paymentId}")]
    [Produces("application/json")]
    [ProducesResponseType(200, Type = typeof(PaymentResponseDto))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetPayment([FromRoute] string paymentId)
    {
        if (!Guid.TryParse(paymentId, out Guid parsedPaymentId))
        {
            return BadRequest("Invalid payment id format. A valid GUID is expected.");
        }

        try
        {
            GetPaymentByIdQuery query = new(parsedPaymentId);
            PaymentResponseDto response = await mediator.Send(query);

            return Ok(response);
        }
        catch (Exception ex)
        {
            return NotFound($"Payment with id {paymentId} not found: {ex.Message}");
        }
    }
}