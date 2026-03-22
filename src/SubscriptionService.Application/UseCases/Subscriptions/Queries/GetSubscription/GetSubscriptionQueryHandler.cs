using SubscriptionService.Application.Abstractions;
using SubscriptionService.Application.Abstractions.Core;
using SubscriptionService.Application.DTOs;
using SharedKernel.Result;

namespace SubscriptionService.Application.UseCases.Subscriptions.Queries.GetSubscription;

public class GetSubscriptionQueryHandler : IQueryHandler<GetSubscriptionQuery, SubscriptionResponse>
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public GetSubscriptionQueryHandler(ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository
                                  ?? throw new ArgumentNullException(nameof(subscriptionRepository));
    }

    public async Task<Result<SubscriptionResponse>> Handle(
        GetSubscriptionQuery query,
        CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository
            .GetByIdAsync(query.SubscriptionId, cancellationToken)
            .ConfigureAwait(false);

        if (subscription is null)
            return Result<SubscriptionResponse>.Failure(
                Error.NotFound($"Подписка с ID '{query.SubscriptionId}' не найдена."));

        return Result<SubscriptionResponse>.Success(new SubscriptionResponse(
            subscription.Id,
            subscription.UserId,
            subscription.PlanId,
            subscription.Status,
            subscription.CurrentPeriodEnd,
            subscription.CancelAtPeriodEnd,
            subscription.TrialEnd,
            subscription.CreatedWhen,
            subscription.Invoices.Select(i => new InvoiceResponse(
                i.Id,
                i.Amount.Value,
                i.Status,
                i.DueDate,
                i.CreatedWhen,
                i.PaidWhen)).ToList()));
    }
}