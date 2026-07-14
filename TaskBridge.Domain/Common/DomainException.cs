namespace TaskBridge.Domain.Common;

/// <summary>
/// Класс доменных ошибок
/// </summary>
public sealed class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
