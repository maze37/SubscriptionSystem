using Core.Abstractions;
using CSharpFunctionalExtensions;
using SharedKernel;
using SubscriptionService.Application.DTOs;

namespace SubscriptionService.Application.UseCases.Subscriptions.Queries.GetSubscription;

/// <summary>
/// Запрос на получение подписки по ID.
/// </summary>
public record GetSubscriptionQuery(GetSubscriptionRequest Request) : IQuery<Result<SubscriptionResponse, Error>>;
