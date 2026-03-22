using MediatR;
using SharedKernel.Result;

namespace SubscriptionService.Application.Abstractions.Core;

/// <summary>Маркер команды без возвращаемого значения.</summary>
public interface ICommand : IRequest<Result> { }

/// <summary>Маркер команды с возвращаемым значением.</summary>
/// <typeparam name="TResponse">Тип возвращаемых данных.</typeparam>
public interface ICommand<TResponse> : IRequest<Result<TResponse>> { }