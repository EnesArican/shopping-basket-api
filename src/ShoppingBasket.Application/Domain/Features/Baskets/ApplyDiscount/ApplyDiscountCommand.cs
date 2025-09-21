namespace ShoppingBasket.Application.Domain.Features.Baskets.ApplyDiscount;

public record ApplyDiscountCommand(
    Guid BasketId,
    string DiscountCode);
