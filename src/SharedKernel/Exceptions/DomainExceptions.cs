namespace SharedKernel.Exceptions;

/// <summary>
/// Исключение, представляющее ошибку предметной области (бизнес-правила)
/// </summary>
public class DomainException : Exception
{
    /// <summary>
    /// Код ошибки для идентификации типа проблемы (например: "playlist.max_items", "order.invalid")
    /// </summary>
    public string? Code { get; }
    
    /// <summary>
    /// Название поля, вызвавшего ошибку (для валидации)
    /// </summary>
    public string? Field { get; }

    /// <summary>
    /// Пустой конструктор (для совместимости)
    /// </summary>
    public DomainException() { }
    
    /// <summary>
    /// Конструктор с внутренним исключением (для цепочки ошибок)
    /// </summary>
    public DomainException(string message, Exception innerException) { }

    /// <summary>
    /// Конструктор только с сообщением (код ошибки по умолчанию)
    /// </summary>
    public DomainException(string message) : base(message) { }

    /// <summary>
    /// Конструктор с кодом и сообщением
    /// </summary>
    /// <param name="code">Машиночитаемый код ошибки</param>
    /// <param name="message">Понятное описание ошибки</param>
    public DomainException(string code, string message) : base(message)
    {
        Code = code;
    }

    /// <summary>
    /// Конструктор с кодом, сообщением и полем
    /// </summary>
    /// <param name="code">Машиночитаемый код ошибки</param>
    /// <param name="message">Понятное описание ошибки</param>
    /// <param name="field">Поле, вызвавшее ошибку</param>
    public DomainException(string code, string message, string field) : base(message)
    {
        Code = code;
        Field = field;
    }
}