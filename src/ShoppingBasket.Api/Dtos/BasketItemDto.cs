namespace ShoppingBasket.Api.Dtos;

public record BasketItemDto(
    Guid Id,
    ItemDto Item,
    int Quantity,
    bool IsDiscounted,
    int? DiscountPercentage,
    decimal TotalPrice);
