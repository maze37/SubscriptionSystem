using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Result;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.UseCases.Plans.Commands.CreatePlan;
using SubscriptionService.Application.UseCases.Plans.Queries.GetActivePlans;
using SubscriptionService.Domain.Enums;

namespace SubscriptionService.Web.Controllers;

[ApiController]
[Route("api/plans")]
public sealed class PlansController : ControllerBase
{
    private readonly ISender _sender;

    public PlansController(ISender sender)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    /// <summary>Создать тарифный план.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreatePlanRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreatePlanCommand(
            Guid.NewGuid(),
            request.Name,
            request.Price,
            request.BillingPeriod,
            DateTimeOffset.UtcNow);

        var result = await _sender.Send(command, cancellationToken)
            .ConfigureAwait(false);

        if (result.IsFailure)
        {
            var error = result.Errors.First();
            return error.Type switch
            {
                ErrorType.Validation => BadRequest(result.Errors),
                _ => StatusCode(500, result.Errors)
            };
        }

        return CreatedAtAction(nameof(GetActive), new { id = result.Value }, result.Value);
    }

    /// <summary>Получить все активные планы.</summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IReadOnlyList<PlanResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActive(CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new GetActivePlansQuery(), cancellationToken)
            .ConfigureAwait(false);

        return Ok(result.Value);
    }
}