using ShoppingBasket.Application.Domain.Models;
using ShoppingBasket.Application.Infrastructure.Entities;

namespace ShoppingBasket.Application.Infrastructure.Mappers;

public static class BasketMapper
{
    public static Basket ToDomain(this BasketEntity entity) =>
        new(
            entity.Id,
            entity.Items.Select(i => i.ToDomain()).ToList(),
            entity.DiscountCode);
}
