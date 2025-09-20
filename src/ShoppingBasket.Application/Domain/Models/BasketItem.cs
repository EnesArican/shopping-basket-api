namespace ShoppingBasket.Application.Domain.Models;

public record BasketItem(
Guid Id,
Item Item,
int Quantity,
bool IsDiscounted,
int? DiscountPercentage,
decimal TotalPrice);

