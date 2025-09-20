using ShoppingBasket.Application.Domain.Models;
using ShoppingBasket.Application.Infrastructure.Entities;

namespace ShoppingBasket.Application.Infrastructure.Mappers;

public static class BasketMapper
{
    public static Basket ToDomain(this BasketEntity entity) =>
        new(
            entity.Id,
            [.. entity.BasketItems.Select(i => i.ToDomain())],
            entity.DiscountCode);

    public static BasketEntity ToEntity(this Basket basket) =>
        new(
            basket.Id,
            [.. basket.Items.Select(i => i.ToEntity())],
            basket.DiscountCode);
}
