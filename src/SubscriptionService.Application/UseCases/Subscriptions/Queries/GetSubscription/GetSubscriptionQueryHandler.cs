using SubscriptionService.Application.Abstractions;
using SubscriptionService.Application.Abstractions.Core;
using SubscriptionService.Application.DTOs;
using SharedKernel.Result;

namespace SubscriptionService.Application.UseCases.Subscriptions.Queries.GetSubscription;

/// <summary>
/// Обработчик запроса GetSubscriptionQuery.
/// Возвращает подписку с историей счетов.
/// </summary>
public class GetSubscriptionQueryHandler : IQueryHandler<GetSubscriptionQuery, SubscriptionResponse>
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public GetSubscriptionQueryHandler(ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository
                                  ?? throw new ArgumentNullException(nameof(subscriptionRepository));
    }

    /// <inheritdoc/>
    public async Task<Result<SubscriptionResponse, Error>> Handle(
        GetSubscriptionQuery query,
        CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository
            .GetByIdAsync(query.SubscriptionId, cancellationToken)
            .ConfigureAwait(false);

        if (subscription is null)
            return Result<SubscriptionResponse, Error>.Failure(
                Error.NotFound($"Подписка с ID '{query.SubscriptionId}' не найдена."));

        return Result<SubscriptionResponse, Error>.Success(subscription.ToResponse());
    }
}
