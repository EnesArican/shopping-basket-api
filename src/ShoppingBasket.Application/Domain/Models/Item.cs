namespace ShoppingBasket.Application.Domain.Models;

public record Item(
    Guid Id,
    string Name,
    decimal Price);

