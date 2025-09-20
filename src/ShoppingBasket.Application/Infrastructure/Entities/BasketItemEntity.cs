namespace ShoppingBasket.Application.Infrastructure.Entities;

public class BasketItemEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public ItemEntity Item { get; init; } = default!;
    public int Quantity { get; set; } = 1;
    public bool IsDiscounted { get; set; } = false;
    public int? DiscountPercentage { get; set; }
    public decimal TotalPrice => Item.Price * Quantity;

    public void Update(int quantity, bool isDiscounted, int? discountPercentage)
    {
        Quantity += quantity;
        IsDiscounted = isDiscounted;
        DiscountPercentage = discountPercentage;
    }
}
