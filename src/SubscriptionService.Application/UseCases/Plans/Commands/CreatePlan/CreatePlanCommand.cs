using Core.Abstractions;
using SubscriptionService.Application.DTOs;

namespace SubscriptionService.Application.UseCases.Plans.Commands.CreatePlan;

/// <summary>
/// Команда создания нового тарифного плана.
/// </summary>
/// <param name="Request">Данные нового тарифного плана.</param>
public record CreatePlanCommand(CreatePlanRequest Request) : ICommand;
