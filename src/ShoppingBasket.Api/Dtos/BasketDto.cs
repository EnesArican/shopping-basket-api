namespace ShoppingBasket.Api.Dtos;

public record BasketDto(
     Guid Id,
     List<BasketItemDto> Items,
     string? DiscountCode,
     string? ShippingCountry,
     decimal? ShippingCost);
