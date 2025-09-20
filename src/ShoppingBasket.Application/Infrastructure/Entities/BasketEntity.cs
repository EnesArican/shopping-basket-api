namespace ShoppingBasket.Application.Infrastructure.Entities;

public class BasketEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public List<BasketItemEntity> BasketItems { get; } = [];
    public string? DiscountCode { get; set; }
}
