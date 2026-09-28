using CSharpFunctionalExtensions;
using SharedKernel;
using Core.Abstractions;
using SubscriptionService.Application.Abstractions;
using SubscriptionService.Application.DTOs;

namespace SubscriptionService.Application.UseCases.Subscriptions.Queries.GetSubscription;

/// <summary>
/// Обработчик запроса GetSubscriptionQuery.
/// Возвращает подписку с историей счетов.
/// </summary>
public class GetSubscriptionQueryHandler : IQueryHandlerWithResult<GetSubscriptionQuery, SubscriptionResponse>
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public GetSubscriptionQueryHandler(ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    /// <inheritdoc/>
    public async Task<Result<SubscriptionResponse, Error>> HandleAsync(
        GetSubscriptionQuery query,
        CancellationToken cancellationToken)
    {
        if (query.Request.SubscriptionId == Guid.Empty)
            return GeneralErrors.ValueIsInvalid(nameof(query.Request.SubscriptionId));

        var subscriptionResult = await _subscriptionRepository
            .GetByIdAsync(query.Request.SubscriptionId, cancellationToken);

        if (subscriptionResult.IsFailure)
            return subscriptionResult.Error;

        var subscription = subscriptionResult.Value;
        var invoices = subscription.Invoices
            .Select(invoice => new InvoiceResponse(
                invoice.Id,
                invoice.Amount.Value,
                invoice.Status,
                invoice.DueDate,
                invoice.CreatedWhen,
                invoice.PaidWhen))
            .ToList();

        return new SubscriptionResponse(
            subscription.Id,
            subscription.UserId,
            subscription.PlanId,
            subscription.Status,
            subscription.CurrentPeriodEnd,
            subscription.CancelAtPeriodEnd,
            subscription.TrialEnd,
            subscription.CreatedWhen,
            invoices);
    }
}
