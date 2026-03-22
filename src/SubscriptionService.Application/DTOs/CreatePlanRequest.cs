using SubscriptionService.Domain.Enums;

namespace SubscriptionService.Application.DTOs;

/// <summary>HTTP request модель для создания тарифного плана.</summary>
/// <param name="Name">Название плана.</param>
/// <param name="Price">Цена плана - должна быть больше нуля.</param>
/// <param name="BillingPeriod">Период оплаты - Monthly или Yearly.</param>
public record CreatePlanRequest(string Name, decimal Price, BillingPeriod BillingPeriod);