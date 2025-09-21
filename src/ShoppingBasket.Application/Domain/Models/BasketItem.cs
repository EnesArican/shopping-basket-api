namespace ShoppingBasket.Application.Domain.Models;

public class BasketItem(
  Guid id, 
  Item item, 
  int quantity, 
  bool isDiscounted, 
  int? discountPercentage)
{
    public Guid Id { get; init; } = id;
    public Item Item { get; init; } = item;
    public int Quantity { get; private set; } = quantity;
    public bool IsDiscounted { get; private set; } = isDiscounted;
    public int? DiscountPercentage { get; private set; } = discountPercentage;
    public decimal TotalPrice { get; private set; } = CalculateTotalPrice(item.Price, quantity, isDiscounted, discountPercentage);

    public void Update(int quantity, bool isDiscounted, int? discountPercentage)
    {
        Quantity += quantity;
        IsDiscounted = isDiscounted;
        DiscountPercentage = discountPercentage;
        TotalPrice = CalculateTotalPrice();
    }

    public void DecrementQuantity(int amount = 1)
    {
        Quantity = Math.Max(0, Quantity - amount);
        TotalPrice = CalculateTotalPrice();
    }

    private decimal CalculateTotalPrice()
    {
        return CalculateTotalPrice(Item.Price, Quantity, IsDiscounted, DiscountPercentage);
    }

    public static decimal CalculateTotalPrice(decimal itemPrice, int quantity, bool isDiscounted, int? discountPercentage)
    {
        var baseTotal = itemPrice * quantity;
        
        if (isDiscounted && discountPercentage.HasValue)
        {
            var discountAmount = baseTotal * (discountPercentage.Value / 100m);
            return baseTotal - discountAmount;
        }

        return baseTotal;
    }
}

