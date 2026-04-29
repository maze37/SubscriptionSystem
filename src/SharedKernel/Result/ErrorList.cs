namespace SharedKernel.Result;

using System.Collections;

/// <summary>
/// Коллекция ошибок домена/приложения.
/// Удобна когда нужно вернуть сразу несколько ошибок валидации.
/// </summary>
public class ErrorList : IEnumerable<Error>
{
    private readonly List<Error> _errors;

    /// <summary>Создать список ошибок.</summary>
    public ErrorList(IEnumerable<Error> errors)
    {
        _errors = [..errors];
    }
    
    public IEnumerator<Error> GetEnumerator()
    {
        return _errors.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    public static implicit operator ErrorList(List<Error> errors)
        => new(errors);
    
    public static implicit operator ErrorList(Error error)
        => new([error]);

    /// <summary>Вернуть только для чтения.</summary>
    public IReadOnlyList<Error> AsReadOnly() => _errors.AsReadOnly();
}
