using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Result;
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
    private readonly ISender _sender;

    public SubscriptionsController(ISender sender)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    /// <summary>Создать подписку.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        [FromBody] CreateSubscriptionRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateSubscriptionCommand(
            Guid.NewGuid(),
            request.UserId,
            request.PlanId,
            Guid.NewGuid(),
            request.WithTrial,
            DateTimeOffset.UtcNow);

        var result = await _sender.Send(command, cancellationToken)
            .ConfigureAwait(false);

        if (result.IsFailure)
        {
            var error = result.Errors.First();
            return error.Type switch
            {
                ErrorType.NotFound => NotFound(result.Errors),
                ErrorType.Conflict => Conflict(result.Errors),
                ErrorType.Validation => BadRequest(result.Errors),
                _ => StatusCode(500, result.Errors)
            };
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    /// <summary>Получить подписку по ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SubscriptionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new GetSubscriptionQuery(id), cancellationToken)
            .ConfigureAwait(false);

        if (result.IsFailure)
            return NotFound(result.Errors);

        return Ok(result.Value);
    }

    /// <summary>Отменить подписку.</summary>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Cancel(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new CancelSubscriptionCommand(id, DateTimeOffset.UtcNow);

        var result = await _sender.Send(command, cancellationToken)
            .ConfigureAwait(false);

        if (result.IsFailure)
        {
            var error = result.Errors.First();
            return error.Type switch
            {
                ErrorType.NotFound => NotFound(result.Errors),
                ErrorType.Conflict => Conflict(result.Errors),
                _ => StatusCode(500, result.Errors)
            };
        }

        return NoContent();
    }

    /// <summary>Сменить тарифный план.</summary>
    [HttpPost("{id:guid}/change-plan")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ChangePlan(
        Guid id,
        [FromBody] ChangePlanRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new ChangePlanCommand(
            id,
            request.NewPlanId,
            Guid.NewGuid(),
            DateTimeOffset.UtcNow);

        var result = await _sender.Send(command, cancellationToken)
            .ConfigureAwait(false);

        if (result.IsFailure)
        {
            var error = result.Errors.First();
            return error.Type switch
            {
                ErrorType.NotFound => NotFound(result.Errors),
                ErrorType.Conflict => Conflict(result.Errors),
                ErrorType.Validation => BadRequest(result.Errors),
                _ => StatusCode(500, result.Errors)
            };
        }

        return NoContent();
    }

    /// <summary>Активировать подписку после оплаты счёта.</summary>
    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Activate(
        Guid id,
        [FromBody] ActivateSubscriptionRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new ActivateSubscriptionCommand(
            id,
            request.InvoiceId,
            DateTimeOffset.UtcNow);

        var result = await _sender.Send(command, cancellationToken)
            .ConfigureAwait(false);

        if (result.IsFailure)
        {
            var error = result.Errors.First();
            return error.Type switch
            {
                ErrorType.NotFound => NotFound(result.Errors),
                _ => StatusCode(500, result.Errors)
            };
        }

        return NoContent();
    }
}

