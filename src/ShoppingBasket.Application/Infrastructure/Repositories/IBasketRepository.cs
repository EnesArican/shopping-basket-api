using ShoppingBasket.Application.Components.Utils;
using ShoppingBasket.Application.Domain.Models;

namespace ShoppingBasket.Application.Infrastructure.Repositories;

public interface IBasketRepository
{
    Task<DataResult<Basket>> CreateBasketAsync(CancellationToken token);
}
