using System.Diagnostics.CodeAnalysis;

namespace ShoppingBasket.Application.Components.Utils;

public record DataResult<T> where T : notnull
{
    public T? Data { get; init; } = default!;
    public string? ErrorCode { get; init; }
    public bool ObsoleteIsValid => !Equals(Data, default(T?));

    public static DataResult<T> Success(T data) =>
        new() { Data = data };

    public static DataResult<T> Failure(string? errorCode) =>
        new() { ErrorCode = errorCode };

    public bool IsValid([NotNullWhen(true)] out T? data)
    {
        data = Data;
        return !Equals(Data, default(T?));
    }
}
