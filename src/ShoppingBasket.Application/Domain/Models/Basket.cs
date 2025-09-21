namespace ShoppingBasket.Application.Domain.Models;

public class Basket(
    Guid id,
    List<BasketItem> items,
    string? discountCode)
{
    public Guid Id { get; } = id;
    public List<BasketItem> Items { get; private set; } = items;
    public string? DiscountCode { get; private set; } = discountCode;

    public void RemoveItem(Guid basketItemId, int quantity = 1)
    {
        var basketItem = Items.FirstOrDefault(i => i.Id == basketItemId);
        if (basketItem == null) return;

        basketItem.DecrementQuantity(quantity);
        
        // Remove the item completely if quantity reaches 0
        if (basketItem.Quantity <= 0)
        {
            Items = [.. Items.Where(i => i.Id != basketItemId)];
        }
    }

    public void SetItems(List<BasketItem> items)
    {
        Items = items;
    }

    public void SetDiscountCode(string discountCode)
    {
        DiscountCode = discountCode;
    }
}
