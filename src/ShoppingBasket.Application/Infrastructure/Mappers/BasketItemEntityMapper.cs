using ShoppingBasket.Application.Domain.Models;
using ShoppingBasket.Application.Infrastructure.Entities;

namespace ShoppingBasket.Application.Infrastructure.Mappers;

public static class BasketItemEntityMapper
{
    public static BasketItem ToDomain(this BasketItemEntity basketItem) =>
        new(
            basketItem.Id,
            basketItem.Item.ToDomain(),
            basketItem.Quantity,
            basketItem.IsDiscounted,
            basketItem.DiscountPercentage);

    public static BasketItemEntity ToEntity(this BasketItem basketItem) =>
        new(
            basketItem.Id,
            basketItem.Item.ToEntity(),
            basketItem.Quantity,
            basketItem.IsDiscounted,
            basketItem.DiscountPercentage,
            basketItem.TotalPrice);
}
