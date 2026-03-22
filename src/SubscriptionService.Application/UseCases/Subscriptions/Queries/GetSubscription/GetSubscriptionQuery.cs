using SubscriptionService.Application.Abstractions.Core;
using SubscriptionService.Application.DTOs;

namespace SubscriptionService.Application.UseCases.Subscriptions.Queries.GetSubscription;

public record GetSubscriptionQuery(Guid SubscriptionId) : IQuery<SubscriptionResponse>;