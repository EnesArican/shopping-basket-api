namespace ShoppingBasket.Application.Domain.Features.Baskets.SetShipping;

public record SetShippingCommand(
    Guid BasketId,
    string Country);
