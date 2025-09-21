namespace ShoppingBasket.Application.Domain.Features.Baskets.RemoveBasketItem;

public record RemoveBasketItemCommand(
    Guid BasketId,
    Guid ItemId);
