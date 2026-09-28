using Core.Abstractions;
using Framework.ResponseExtensions;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.UseCases.Subscriptions.Commands.ActivateSubscription;
using SubscriptionService.Application.UseCases.Subscriptions.Commands.CancelSubscription;
using SubscriptionService.Application.UseCases.Subscriptions.Commands.ChangePlan;
using SubscriptionService.Application.UseCases.Subscriptions.Commands.CreateSubscription;
using SubscriptionService.Application.UseCases.Subscriptions.Queries.GetSubscription;

namespace SubscriptionService.Web.Controllers;

[ApiController]
[Route("api/subscriptions")]
public sealed class SubscriptionsController : ControllerBase
{
    private readonly ICommandHandler<CreateSubscriptionCommand, CreateSubscriptionResponse> _createSubscriptionHandler;
    private readonly IQueryHandlerWithResult<GetSubscriptionQuery, SubscriptionResponse> _getSubscriptionHandler;
    private readonly ICommandHandler<CancelSubscriptionCommand, CancelSubscriptionResponse> _cancelSubscriptionHandler;
    private readonly ICommandHandler<ChangePlanCommand, ChangePlanResponse> _changePlanHandler;
    private readonly ICommandHandler<ActivateSubscriptionCommand, ActivateSubscriptionResponse> _activateSubscriptionHandler;

    public SubscriptionsController(
        ICommandHandler<CreateSubscriptionCommand, CreateSubscriptionResponse> createSubscriptionHandler,
        IQueryHandlerWithResult<GetSubscriptionQuery, SubscriptionResponse> getSubscriptionHandler,
        ICommandHandler<CancelSubscriptionCommand, CancelSubscriptionResponse> cancelSubscriptionHandler,
        ICommandHandler<ChangePlanCommand, ChangePlanResponse> changePlanHandler,
        ICommandHandler<ActivateSubscriptionCommand, ActivateSubscriptionResponse> activateSubscriptionHandler)
    {
        _createSubscriptionHandler = createSubscriptionHandler;
        _getSubscriptionHandler = getSubscriptionHandler;
        _cancelSubscriptionHandler = cancelSubscriptionHandler;
        _changePlanHandler = changePlanHandler;
        _activateSubscriptionHandler = activateSubscriptionHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateSubscriptionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateSubscriptionCommand(request);
        var response = await _createSubscriptionHandler.HandleAsync(command, cancellationToken);

        if (response.IsFailure)
            return response.Error.ToResponse();

        return Ok(Envelope.Ok(response.Value));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetSubscriptionQuery(new GetSubscriptionRequest(id));
        var response = await _getSubscriptionHandler.HandleAsync(query, cancellationToken);

        if (response.IsFailure)
            return response.Error.ToResponse();

        return Ok(Envelope.Ok(response.Value));
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var response = await _cancelSubscriptionHandler.HandleAsync(
            new CancelSubscriptionCommand(new CancelSubscriptionRequest(id)), cancellationToken);

        if (response.IsFailure)
            return response.Error.ToResponse();

        return Ok(Envelope.Ok(response.Value));
    }

    [HttpPost("{id:guid}/change-plan")]
    public async Task<IActionResult> ChangePlan(
        Guid id,
        [FromBody] ChangePlanRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _changePlanHandler.HandleAsync(
            new ChangePlanCommand(id, request), cancellationToken);

        if (response.IsFailure)
            return response.Error.ToResponse();

        return Ok(Envelope.Ok(response.Value));
    }

    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> Activate(
        Guid id,
        [FromBody] ActivateSubscriptionRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _activateSubscriptionHandler.HandleAsync(
            new ActivateSubscriptionCommand(id, request), cancellationToken);

        if (response.IsFailure)
            return response.Error.ToResponse();

        return Ok(Envelope.Ok(response.Value));
    }
}
