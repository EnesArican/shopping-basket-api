namespace ShoppingBasket.Application.Domain.Models;

public class Basket(
    Guid id,
    List<BasketItem> items,
    string? discountCode)
{
    public Guid Id { get; } = id;
    public List<BasketItem> Items { get; private set; } = items;
    public string? DiscountCode { get; set; } = discountCode;

    public void RemoveItem(Guid itemId)
    {
        Items = [.. Items.Where(i => i.Id != itemId)];
    }

    public void SetItems(List<BasketItem> items)
    {
        Items = items;
    }
}
