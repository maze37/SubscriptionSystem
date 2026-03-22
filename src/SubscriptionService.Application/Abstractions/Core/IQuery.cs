using MediatR;
using SharedKernel.Result;

namespace SubscriptionService.Application.Abstractions.Core;

/// <summary>
/// Маркер запроса — не изменяет состояние системы, только читает данные.
/// </summary>
/// <typeparam name="TResponse">Тип возвращаемых данных.</typeparam>
public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }