namespace SharedKernel.Result;

/// <summary>
/// Результат операции без полезных данных.
/// Используется для команд, где важно только "успех или ошибка".
/// </summary>
/// <typeparam name="TError">Тип ошибки.</typeparam>
public sealed class Result<TError>
{
    private Result(bool isSuccess, TError? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>Операция завершилась успешно.</summary>
    public bool IsSuccess { get; }
    /// <summary>Операция завершилась с ошибкой.</summary>
    public bool IsFailure => !IsSuccess;
    /// <summary>Ошибка операции, если IsFailure = true.</summary>
    public TError? Error { get; }

    /// <summary>Создать успешный результат.</summary>
    public static Result<TError> Success() => new(true, default);
    /// <summary>Создать результат с ошибкой.</summary>
    public static Result<TError> Failure(TError error) => new(false, error);
}

/// <summary>
/// Результат операции с полезными данными.
/// </summary>
/// <typeparam name="TValue">Тип успешного значения.</typeparam>
/// <typeparam name="TError">Тип ошибки.</typeparam>
public sealed class Result<TValue, TError>
{
    private Result(TValue? value, bool isSuccess, TError? error)
    {
        Value = value;
        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>Значение операции, если IsSuccess = true.</summary>
    public TValue? Value { get; }
    /// <summary>Операция завершилась успешно.</summary>
    public bool IsSuccess { get; }
    /// <summary>Операция завершилась с ошибкой.</summary>
    public bool IsFailure => !IsSuccess;
    /// <summary>Ошибка операции, если IsFailure = true.</summary>
    public TError? Error { get; }

    /// <summary>Создать успешный результат.</summary>
    public static Result<TValue, TError> Success(TValue value) => new(value, true, default);
    /// <summary>Создать результат с ошибкой.</summary>
    public static Result<TValue, TError> Failure(TError error) => new(default, false, error);
}
