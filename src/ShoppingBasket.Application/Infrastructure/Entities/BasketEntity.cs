namespace ShoppingBasket.Application.Infrastructure.Entities;

public record BasketEntity(
    Guid Id,
    List<BasketItemEntity> BasketItems,
    string? DiscountCode,
    string? ShippingCountry,
    decimal? ShippingCost);
