using ShoppingBasket.Api.Dtos;
using ShoppingBasket.Application.Domain.Models;

namespace ShoppingBasket.Api.Mappers;

public static class ItemDtoMapper
{
    public static ItemDto ToDto(this Item item) =>
        new(
            item.Id,
            item.Name,
            item.Price);
}
