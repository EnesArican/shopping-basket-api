using ShoppingBasket.Api.Dtos;
using ShoppingBasket.Application.Domain.Models;

namespace ShoppingBasket.Api.Mappers;

public static class BasketDtoMapper
{
    public static BasketDto ToDto(this Basket basket) =>
        new(
            basket.Id,
            basket.Items.Select(i => i.ToDto()).ToList(),
            basket.DiscountCode,
            basket.ShippingCountry,
            basket.ShippingCost);
}
