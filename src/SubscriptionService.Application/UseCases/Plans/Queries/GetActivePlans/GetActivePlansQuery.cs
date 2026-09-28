using Core.Abstractions;
using CSharpFunctionalExtensions;
using SharedKernel;
using SubscriptionService.Application.DTOs;

namespace SubscriptionService.Application.UseCases.Plans.Queries.GetActivePlans;

/// <summary>
/// Запрос на получение всех активных тарифных планов.
/// Параметров нет - валидатор не нужен.
/// </summary>
public record GetActivePlansQuery(GetActivePlansRequest Request) : IQuery<Result<GetActivePlansResponse, Error>>;
