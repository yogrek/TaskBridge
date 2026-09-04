namespace TaskBridge.Contracts.Common;

public sealed record ErrorResponse(
    string Title,
    string Detail,
    int Status,
    string? Type,
    string? TraceId);
