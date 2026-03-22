using SubscriptionService.Application.Abstractions.Core;
using SubscriptionService.Application.DTOs;

namespace SubscriptionService.Application.UseCases.Subscriptions.Queries.GetSubscription;

/// <summary>Запрос на получение подписки по ID.</summary>
/// <param name="SubscriptionId">ID подписки.</param>
public record GetSubscriptionQuery(Guid SubscriptionId) : IQuery<SubscriptionResponse>;