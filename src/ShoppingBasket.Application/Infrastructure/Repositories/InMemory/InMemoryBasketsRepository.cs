using Microsoft.Extensions.Logging;
using ShoppingBasket.Application.Domain.Models;
using ShoppingBasket.Application.Components.Utils;
using ShoppingBasket.Application.Infrastructure.Entities;
using ShoppingBasket.Application.Infrastructure.Mappers;

namespace ShoppingBasket.Application.Infrastructure.Repositories.InMemory;

public class InMemoryBasketsRepository(
    ILogger<InMemoryBasketsRepository> logger) : IBasketsRepository
{
    private readonly Dictionary<Guid, BasketEntity> _baskets = [];

    public Task<DataResult<Basket>> CreateBasketAsync(CancellationToken token)
    {
        var basket = new BasketEntity();
        _baskets[basket.Id] = basket;

        return Task.FromResult(DataResult<Basket>.Success(basket.ToDomain()));
    }

    public Task<DataResult<Basket>> GetBasketByIdAsync(Guid basketId, CancellationToken token)
    {
        if (!_baskets.TryGetValue(basketId, out var basketEntity))
            return Task.FromResult(DataResult<Basket>.Failure(ErrorCodes.BasketNotFound));

        return Task.FromResult(DataResult<Basket>.Success(basketEntity.ToDomain()));
    }

    public Task<DataResult<Basket>> UpdateBasketAsync(Basket basket, CancellationToken token)
    {
        if (!_baskets.ContainsKey(basket.Id))
            return Task.FromResult(DataResult<Basket>.Failure(ErrorCodes.BasketNotFound));

        // Replace the entire entity with a new one
        _baskets[basket.Id] = basket.ToEntity();

        return Task.FromResult(DataResult<Basket>.Success(basket));
    }
}
