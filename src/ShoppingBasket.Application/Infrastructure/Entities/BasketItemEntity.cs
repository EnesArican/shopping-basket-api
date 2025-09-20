namespace ShoppingBasket.Application.Infrastructure.Entities;

public class BasketItemEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public ItemEntity Product { get; init; } = default!;
    public int Quantity { get; set; } = 1;
    public bool IsDiscounted { get; set; } = false;
    public int? DiscountPercentage { get; set; }
    public decimal TotalPrice => Product.Price * Quantity;
}
