namespace fixedhitbox.Shared.Results.Mappers;

public sealed class MapperResult<T>
{
    public bool Success { get; }
    public T? Value { get; }
    public string? Error { get; }

    private MapperResult(T value)
    {
        Success = true;
        Value = value;
        Error = null;
    }

    private MapperResult(string error)
    {
        Success = false;
        Error = error;
        Value = default;
    }

    public static MapperResult<T> Ok(T value) => new(value);
    public static MapperResult<T> Fail(string error) => new(error);
}