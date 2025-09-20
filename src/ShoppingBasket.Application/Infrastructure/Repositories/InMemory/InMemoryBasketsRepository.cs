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
}
