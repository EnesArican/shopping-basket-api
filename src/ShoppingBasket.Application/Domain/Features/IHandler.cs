namespace ShoppingBasket.Application.Domain.Features;

public interface IHandler<T, TResult>
{
    Task<TResult> ExecuteAsync(T request, CancellationToken token);
}