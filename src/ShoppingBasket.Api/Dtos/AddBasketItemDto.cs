namespace ShoppingBasket.Api.Dtos;

public record AddBasketItemDto(
    Guid ItemId,
    int Quantity,
    bool IsDiscounted = false,
    int? DiscountPercentage = null);
