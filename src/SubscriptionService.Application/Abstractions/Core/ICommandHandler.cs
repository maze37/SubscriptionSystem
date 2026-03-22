using MediatR;
using SharedKernel.Result;

namespace SubscriptionService.Application.Abstractions.Core;

/// <summary>
/// Обработчик команды без возвращаемого значения.
/// Реализует IRequestHandler MediatR под капотом.
/// </summary>
/// <typeparam name="TCommand">Тип команды.</typeparam>
public interface ICommandHandler<TCommand>
    : IRequestHandler<TCommand, Result>
    where TCommand : ICommand { }

/// <summary>
/// Обработчик команды с возвращаемым значением.
/// Реализует IRequestHandler MediatR под капотом.
/// </summary>
/// <typeparam name="TCommand">Тип команды.</typeparam>
/// <typeparam name="TResponse">Тип возвращаемых данных.</typeparam>
public interface ICommandHandler<TCommand, TResponse>
    : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse> { }