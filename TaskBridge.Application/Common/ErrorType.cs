namespace TaskBridge.Application.Common;

/// <summary>
/// Represents ErrorType.
/// </summary>
public enum ErrorType
{
    Validation = 1,
    NotFound = 2,
    Forbidden = 3,
    Conflict = 4,
    Unauthorized = 5,
    Failure = 6,
}
