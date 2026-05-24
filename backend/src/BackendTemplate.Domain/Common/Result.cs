namespace BackendTemplate.Domain.Common;

public enum ErrorKind { Validation, NotFound, Conflict }

public readonly struct Unit
{
    public static readonly Unit Value = default;
}

public sealed class Result<T>
{
    private readonly T? _value;
    private readonly string? _error;
    private readonly ErrorKind _kind;

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public T Value => IsSuccess ? _value! : throw new InvalidOperationException("Cannot access Value on a failed Result.");
    public string Error => IsFailure ? _error! : throw new InvalidOperationException("Cannot access Error on a successful Result.");
    public ErrorKind Kind => IsFailure ? _kind : throw new InvalidOperationException("Cannot access Kind on a successful Result.");

    private Result(T value) { IsSuccess = true; _value = value; }
    private Result(string error, ErrorKind kind) { IsSuccess = false; _error = error; _kind = kind; }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(string error, ErrorKind kind = ErrorKind.Validation) => new(error, kind);
}
