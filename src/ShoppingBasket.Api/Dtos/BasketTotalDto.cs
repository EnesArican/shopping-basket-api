namespace ShoppingBasket.Api.Dtos;

public record BasketTotalDto(
    Guid BasketId,
    decimal SubTotal,
    decimal VatAmount,
    decimal TotalWithVat,
    decimal TotalWithoutVat,
    int TotalItems);
