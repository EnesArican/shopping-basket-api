namespace ShoppingBasket.Application.Domain.Models;

public class BasketItem(
  Guid id, 
  Item item, 
  int quantity, 
  bool isDiscounted, 
  int? discountPercentage, 
  decimal totalPrice)
{
    public Guid Id { get; init; } = id;
    public Item Item { get; init; } = item;
    public int Quantity { get; private set; } = quantity;
    public bool IsDiscounted { get; private set; } = isDiscounted;
    public int? DiscountPercentage { get; private set; } = discountPercentage;
    public decimal TotalPrice { get; private set; } = totalPrice;

    public void Update(int quantity, bool isDiscounted, int? discountPercentage)
    {
        Quantity += quantity;
        IsDiscounted = isDiscounted;
        DiscountPercentage = discountPercentage;
        TotalPrice = Item.Price * Quantity;
    }
}

