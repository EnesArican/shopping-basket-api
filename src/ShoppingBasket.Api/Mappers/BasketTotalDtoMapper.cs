using ShoppingBasket.Api.Dtos;
using ShoppingBasket.Application.Domain.Features.Baskets.GetBasketTotal;

namespace ShoppingBasket.Api.Mappers;

public static class BasketTotalDtoMapper
{
    public static BasketTotalDto ToDto(this BasketTotalResult basketTotal) =>
        new(
            basketTotal.BasketId,
            basketTotal.SubTotal,
            basketTotal.ShippingCost,
            basketTotal.VatAmount,
            basketTotal.TotalWithVat,
            basketTotal.TotalWithoutVat,
            basketTotal.TotalItems);
}
