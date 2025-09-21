namespace ShoppingBasket.Api.Dtos;

public record BasketTotalDto(
    Guid BasketId,
    decimal SubTotal,
    decimal ShippingCost,
    decimal VatAmount,
    decimal TotalWithVat,
    decimal TotalWithoutVat,
    int TotalItems);
