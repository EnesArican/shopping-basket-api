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
            basketItem.DiscountPercentage,
            basketItem.TotalPrice);

    public static BasketItemEntity ToEntity(this BasketItem basketItem) =>
        new()
        {
            Id = basketItem.Id,
            Item = basketItem.Item.ToEntity(),
            Quantity = basketItem.Quantity,
            IsDiscounted = basketItem.IsDiscounted,
            DiscountPercentage = basketItem.DiscountPercentage,
            TotalPrice = basketItem.TotalPrice
        };
}
