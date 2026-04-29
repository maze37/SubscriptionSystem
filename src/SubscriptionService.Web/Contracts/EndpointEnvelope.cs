using SharedKernel.Result;

namespace SubscriptionService.Web.Contracts;

/// <summary>
/// Единый формат ответа API.
/// </summary>
/// <typeparam name="T">Тип поля result.</typeparam>
public sealed record EndpointEnvelope<T>(
    /// <summary>Данные успешного ответа.</summary>
    T? Result,
    /// <summary>Список ошибок, если запрос завершился неуспешно.</summary>
    IReadOnlyList<Error>? Errors,
    /// <summary>Время формирования ответа в UTC.</summary>
    DateTimeOffset TimeGenerated);
