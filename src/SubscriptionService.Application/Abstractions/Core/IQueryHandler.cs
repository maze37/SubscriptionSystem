using MediatR;
using SharedKernel.Result;

namespace SubscriptionService.Application.Abstractions.Core;

/// <summary>
/// Обработчик запроса — читает данные без изменения состояния системы.
/// </summary>
/// <typeparam name="TQuery">Тип запроса.</typeparam>
/// <typeparam name="TResponse">Тип возвращаемых данных.</typeparam>
public interface IQueryHandler<TQuery, TResponse>
    : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse> { }