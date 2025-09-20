namespace ShoppingBasket.Application.Domain.Models;

public record Basket(
     Guid Id,
     List<BasketItem> Items,
     string? DiscountCode);
