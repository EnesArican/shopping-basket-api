using ShoppingBasket.Application.Domain.Models;
using ShoppingBasket.Application.Infrastructure.Entities;

namespace ShoppingBasket.Application.Infrastructure.Mappers;

public static class ItemEntityMapper
{
    public static Item ToDomain(this ItemEntity item) =>
        new(
            item.Id,
            item.Name,
            item.Price);

    public static ItemEntity ToEntity(this Item item) =>
        new()
        {
            Id = item.Id,
            Name = item.Name,
            Price = item.Price
        };
}
