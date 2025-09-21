using ShoppingBasket.Application.Domain.Models;

namespace ShoppingBasket.Api.Mappers;

public static class BasketItemDtoMapper
{
    public static Dtos.BasketItemDto ToDto(this BasketItem basketItem) =>
        new(
            basketItem.Id,
            basketItem.Item.ToDto(),
            basketItem.Quantity,
            basketItem.IsDiscounted,
            basketItem.DiscountPercentage,
            basketItem.TotalPrice);
}
