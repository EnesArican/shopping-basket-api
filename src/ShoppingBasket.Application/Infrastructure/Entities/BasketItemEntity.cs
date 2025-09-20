namespace ShoppingBasket.Application.Infrastructure.Entities;

public record BasketItemEntity(
    Guid Id,
    ItemEntity Item,
    int Quantity,
    bool IsDiscounted,
    int? DiscountPercentage,
    decimal TotalPrice);
