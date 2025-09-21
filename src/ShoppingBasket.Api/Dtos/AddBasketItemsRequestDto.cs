namespace ShoppingBasket.Api.Dtos;

public record AddBasketItemsRequestDto(
    List<AddBasketItemDto> Items);

public record AddBasketItemDto(
    Guid ItemId,
    int Quantity,
    bool IsDiscounted = false,
    int? DiscountPercentage = null);
