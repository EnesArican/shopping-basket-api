namespace ShoppingBasket.Application.Infrastructure.Entities;

public class ItemEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }

}
