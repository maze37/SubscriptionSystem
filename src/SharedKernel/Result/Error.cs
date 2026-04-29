namespace SharedKernel.Result;

using System.Globalization;

public class Error
{
    private const string separator = "||";

    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);

    public string ErrorCode { get; }
    public string ErrorMessage { get; }
    public ErrorType Type { get; }
    public string? InvalidField { get; }

    private Error(string errorCode, string errorMessage, ErrorType type, string? invalidField = null)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
        Type = type;
        InvalidField = invalidField;
    }

    public static Error Create(
        string errorCode,
        string errorMessage,
        ErrorType type,
        string? invalidField = null) =>
        new(errorCode, errorMessage, type, invalidField);

    public string Serialize()
    {
        return string.Join(separator, ErrorCode, ErrorMessage, Type);
    }

    public static Error Deserialize(string serialized)
    {
        _ = serialized ?? throw new ArgumentNullException(nameof(serialized));

        var parts = serialized.Split(separator);

        if (parts.Length < 3)
        {
            throw new ArgumentException("Invalid serialized format");
        }

        if (Enum.TryParse<ErrorType>(parts[2], out var type) == false)
        {
            throw new ArgumentException("Invalid serialized format");
        }

        return new Error(parts[0], parts[1], type);
    }
    
#pragma warning disable CA1305
    public static Error Validation(string errorMessage, string? invalidField = null) =>
        new(ErrorCodes.ValidationError.ToString(), errorMessage, ErrorType.Validation, invalidField);

    public static Error Validation(string errorCode, string errorMessage, string? invalidField = null) =>
        new(errorCode, errorMessage, ErrorType.Validation, invalidField);
    
    public static Error Failure(string errorMessage) =>
        new(ErrorCodes.InternalServerError.ToString(), errorMessage, ErrorType.Failure);

    public static Error Failure(string errorCode, string errorMessage) =>
        new(errorCode, errorMessage, ErrorType.Failure);
    
    public static Error NotFound(string errorMessage) =>
        new(ErrorCodes.NotFound.ToString(CultureInfo.InvariantCulture), errorMessage, ErrorType.NotFound);

    public static Error NotFound(string errorCode, string errorMessage) =>
        new(errorCode, errorMessage, ErrorType.NotFound);
    
    public static Error Forbidden(string errorMessage) =>
        new(ErrorCodes.Forbidden.ToString(CultureInfo.InvariantCulture), errorMessage, ErrorType.Forbidden);

    public static Error Forbidden(string errorCode, string errorMessage) =>
        new(errorCode, errorMessage, ErrorType.Forbidden);

    public static Error Conflict(string errorMessage) =>
        new(ErrorCodes.Conflict.ToString(), errorMessage, ErrorType.Conflict);

    public static Error Conflict(string errorCode, string errorMessage) =>
        new(errorCode, errorMessage, ErrorType.Conflict);
    
    public static Error Null(string errorMessage, string? invalidField = null) =>
        new(ErrorCodes.BadRequest.ToString(CultureInfo.InvariantCulture), errorMessage, ErrorType.Null, invalidField);
#pragma warning restore CA1305

    public ErrorList ToErrorList() => new([this]);

    public override string ToString()
    {
        return $"ErrorCode: {ErrorCode}.\nErrorMessage:{ErrorMessage}\n{Type}";
    }
}

public enum ErrorType
{
    None,
    Validation,
    NotFound,
    Forbidden,
    Failure,
    Null,
    Conflict
}
