using FluentValidation;
using MediatR;
using SharedKernel.Result;

namespace SubscriptionService.Application.Behaviors;

/// <summary>
/// MediatR Pipeline Behavior — перехватывает каждый запрос до хендлера
/// и запускает все FluentValidation валидаторы для него.
/// Если валидация не прошла — хендлер не вызывается, возвращается failure result.
/// </summary>
public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
            return await next();

        var error = Error.Validation(
            failures.First().ErrorMessage,
            failures.First().PropertyName);

        var responseType = typeof(TResponse);
        
        if (responseType.IsGenericType &&
            responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var resultType = typeof(Result<>).MakeGenericType(typeof(Error));
            var failureMethod = resultType.GetMethod(
                "Failure",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
                new[] { typeof(Error) });

            return (TResponse)failureMethod!.Invoke(null, [error])!;
        }

        if (responseType.IsGenericType &&
            responseType.GetGenericTypeDefinition() == typeof(Result<,>))
        {
            var genericArg = responseType.GetGenericArguments()[0];
            var resultType = typeof(Result<,>).MakeGenericType(genericArg, typeof(Error));
            var failureMethod = resultType.GetMethod(
                "Failure",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
                new[] { typeof(Error) });

            return (TResponse)failureMethod!.Invoke(null, [error])!;
        }

        return await next();
    }
}
