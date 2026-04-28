namespace SharedKernel.Result;

public sealed class Result<TError>
{
    private Result(bool isSuccess, TError? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public TError? Error { get; }

    public static Result<TError> Success() => new(true, default);
    public static Result<TError> Failure(TError error) => new(false, error);
}

public sealed class Result<TValue, TError>
{
    private Result(TValue? value, bool isSuccess, TError? error)
    {
        Value = value;
        IsSuccess = isSuccess;
        Error = error;
    }

    public TValue? Value { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public TError? Error { get; }

    public static Result<TValue, TError> Success(TValue value) => new(value, true, default);
    public static Result<TValue, TError> Failure(TError error) => new(default, false, error);
}
