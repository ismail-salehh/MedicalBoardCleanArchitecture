namespace MedicalBoard.Application.Common;

public class ServiceResult
{
    public bool Succeeded { get; protected set; }
    public string? Error { get; protected set; }
    public IReadOnlyList<string> Errors { get; protected set; } = Array.Empty<string>();

    public static ServiceResult Success() => new() { Succeeded = true };
    public static ServiceResult Failure(string error) => new() { Succeeded = false, Error = error, Errors = [error] };
}

public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; private set; }

    public static ServiceResult<T> Success(T data) => new() { Succeeded = true, Data = data };
    public new static ServiceResult<T> Failure(string error) => new() { Succeeded = false, Error = error, Errors = [error] };
}
