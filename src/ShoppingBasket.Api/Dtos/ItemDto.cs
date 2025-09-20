namespace ShoppingBasket.Api.Dtos;

public record ItemDto(
    Guid Id,
    string Name,
    decimal Price);
