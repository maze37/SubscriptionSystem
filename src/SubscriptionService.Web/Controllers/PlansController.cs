using MediatR;
using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.UseCases.Plans.Commands.CreatePlan;
using SubscriptionService.Application.UseCases.Plans.Queries.GetActivePlans;
using SubscriptionService.Web.Contracts;
using SubscriptionService.Web.Extensions;

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
    [ProducesResponseType(typeof(EndpointEnvelope<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreatePlanRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreatePlanCommand(
            request.Name,
            request.Price,
            request.BillingPeriod);

        var result = await _sender.Send(command, cancellationToken)
            .ConfigureAwait(false);

        return this.FromResult(result, StatusCodes.Status201Created);
    }

    /// <summary>Получить все активные планы.</summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(EndpointEnvelope<IReadOnlyList<PlanResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActive(CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new GetActivePlansQuery(), cancellationToken)
            .ConfigureAwait(false);

        return this.FromResult(result);
    }
}
