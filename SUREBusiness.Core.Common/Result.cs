namespace SUREBusiness.Core.Common;

public sealed class Result<T>
{
    public bool Success => Error == null;
    public string? Error { get; }
    public T? Value { get; }

    private Result(T? value)
    {
        Value = value;
    }

    private Result(string error)
    {
        Error = error;
    }

    public static Result<T> Ok(T? value) => new(value);
    public static Result<T> Fail(string error) => new(error);
}

public sealed class Result
{
    public bool Success => Error == null;
    public string? Error { get; }

    private Result(string? error = null)
    {
        Error = error;
    }

    public static Result Ok() => new();
    public static Result Fail(string error) => new(error);
}
