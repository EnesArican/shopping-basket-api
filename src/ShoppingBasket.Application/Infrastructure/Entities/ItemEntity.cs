namespace ShoppingBasket.Application.Infrastructure.Entities;

public record ItemEntity(
    Guid Id,
    string Name,
    decimal Price);
