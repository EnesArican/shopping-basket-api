namespace ShoppingBasket.Application.Domain.Features.Baskets.GetBasketTotal;

public record BasketTotalResult(
    Guid BasketId,
    decimal SubTotal,
    decimal ShippingCost,
    decimal VatAmount,
    decimal TotalWithVat,
    decimal TotalWithoutVat,
    int TotalItems);
