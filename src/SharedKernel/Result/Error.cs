namespace SharedKernel.Result;

using System.Globalization;

/// <summary>
/// Единая модель ошибки для всех слоев.
/// Содержит код, сообщение, тип и проблемное поле при валидации.
/// </summary>
public class Error
{
    private const string separator = "||";

    /// <summary>Отсутствие ошибки.</summary>
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);

    /// <summary>Машиночитаемый код ошибки.</summary>
    public string ErrorCode { get; }
    /// <summary>Сообщение для клиента или логов.</summary>
    public string ErrorMessage { get; }
    /// <summary>Тип ошибки для маппинга в HTTP-код.</summary>
    public ErrorType Type { get; }
    /// <summary>Поле с некорректным значением, если применимо.</summary>
    public string? InvalidField { get; }

    private Error(string errorCode, string errorMessage, ErrorType type, string? invalidField = null)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
        Type = type;
        InvalidField = invalidField;
    }

    /// <summary>Создать ошибку с произвольными параметрами.</summary>
    public static Error Create(
        string errorCode,
        string errorMessage,
        ErrorType type,
        string? invalidField = null) =>
        new(errorCode, errorMessage, type, invalidField);

    /// <summary>Сериализовать ошибку в строку.</summary>
    public string Serialize()
    {
        return string.Join(separator, ErrorCode, ErrorMessage, Type);
    }

    /// <summary>Восстановить ошибку из строки.</summary>
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

    /// <summary>Преобразовать ошибку в список из одного элемента.</summary>
    public ErrorList ToErrorList() => new([this]);

    public override string ToString()
    {
        return $"ErrorCode: {ErrorCode}.\nErrorMessage:{ErrorMessage}\n{Type}";
    }
}

public enum ErrorType
{
    /// <summary>Ошибки нет.</summary>
    None,
    /// <summary>Ошибка валидации входных данных.</summary>
    Validation,
    /// <summary>Сущность не найдена.</summary>
    NotFound,
    /// <summary>Операция запрещена.</summary>
    Forbidden,
    /// <summary>Системная ошибка выполнения.</summary>
    Failure,
    /// <summary>Передано null или пустое значение.</summary>
    Null,
    /// <summary>Конфликт состояния.</summary>
    Conflict
}
