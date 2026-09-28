using Core.Abstractions;
using Framework.ResponseExtensions;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.UseCases.Plans.Commands.CreatePlan;
using SubscriptionService.Application.UseCases.Plans.Queries.GetActivePlans;

namespace SubscriptionService.Web.Controllers;

[ApiController]
[Route("api/plans")]
public sealed class PlansController : ControllerBase
{
    private readonly ICommandHandler<CreatePlanCommand, CreatePlanResponse> _createPlanHandler;
    private readonly IQueryHandlerWithResult<GetActivePlansQuery, GetActivePlansResponse> _getActivePlansHandler;

    public PlansController(
        ICommandHandler<CreatePlanCommand, CreatePlanResponse> createPlanHandler,
        IQueryHandlerWithResult<GetActivePlansQuery, GetActivePlansResponse> getActivePlansHandler)
    {
        _createPlanHandler = createPlanHandler;
        _getActivePlansHandler = getActivePlansHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePlanRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreatePlanCommand(request);
        var response = await _createPlanHandler.HandleAsync(command, cancellationToken);

        if (response.IsFailure)
            return response.Error.ToResponse();

        return Ok(Envelope.Ok(response.Value));
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive(CancellationToken cancellationToken)
    {
        var query = new GetActivePlansQuery(new GetActivePlansRequest());
        var response = await _getActivePlansHandler.HandleAsync(query, cancellationToken);

        if (response.IsFailure)
            return response.Error.ToResponse();

        return Ok(Envelope.Ok(response.Value));
    }
}
