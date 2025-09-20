namespace ShoppingBasket.Application.Domain.Features.Basket.AddBasketItems;

public record AddBasketItemsCommand(
    Guid BasketId,
    List<BasketItemRequest> Items);

public record BasketItemRequest(
    Guid ItemId,
    int Quantity,
    bool IsDiscounted = false,
    int? DiscountPercentage = null);
