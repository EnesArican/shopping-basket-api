using ShoppingBasket.Application.Components.Utils;
using ShoppingBasket.Application.Infrastructure.Repositories;
using ShoppingBasket.Application.Domain.Models;

namespace ShoppingBasket.Application.Domain.Features.Baskets.AddBasketItems;

using Result = DataResult<Models.Basket>;

public class AddBasketItemsHandler(
    IBasketsRepository basketsRepository,
    IItemsRepository itemsRepository) : IHandler<AddBasketItemsCommand, Result>
{
    public async Task<Result> ExecuteAsync(AddBasketItemsCommand request, CancellationToken token)
    {
        // Validate that we have items to add
        if (request.Items == null || !request.Items.Any())
            return Result.Failure(ErrorCodes.InvalidRequest);

        // Validate each item request
        foreach (var itemRequest in request.Items)
        {
            if (itemRequest.Quantity <= 0)
                return Result.Failure(ErrorCodes.InvalidQuantity);

            if (itemRequest.IsDiscounted && (itemRequest.DiscountPercentage == null || itemRequest.DiscountPercentage <= 0 || itemRequest.DiscountPercentage > 100))
                return Result.Failure(ErrorCodes.InvalidDiscountPercentage);
        }

        // Get the basket
        var basketResult = await basketsRepository.GetBasketByIdAsync(request.BasketId, token);
        if (!basketResult.IsValid(out var basket))
            return Result.Failure(ErrorCodes.BasketNotFound);

        // Get all items from the catalog
        var itemIds = request.Items.Select(i => i.ItemId).Distinct().ToList();
        var items = new Dictionary<Guid, Models.Item>();
        
        foreach (var itemId in itemIds)
        {
            var itemResult = await itemsRepository.GetItemByIdAsync(itemId, token);
            if (!itemResult.IsValid(out var item))
                return Result.Failure(ErrorCodes.ItemNotFound);
            
            items[itemId] = item!;
        }

        // Process all items and build updated basket
        var updatedItems = basket!.Items.ToList();

        foreach (var itemRequest in request.Items)
        {
            var item = items[itemRequest.ItemId];
            var existingItem = updatedItems.FirstOrDefault(i => i.Item.Id == itemRequest.ItemId);

            if (existingItem != null)
            {
                // Update existing item
                var updatedItem = existingItem with {
                    Quantity = existingItem.Quantity + itemRequest.Quantity,
                    IsDiscounted = itemRequest.IsDiscounted,
                    DiscountPercentage = itemRequest.DiscountPercentage,
                    TotalPrice = existingItem.Item.Price * (existingItem.Quantity + itemRequest.Quantity)
                };

                var index = updatedItems.IndexOf(existingItem);
                updatedItems[index] = updatedItem;
            }
            else
            {
                // Add new item
                var newBasketItem = new BasketItem(
                    Guid.NewGuid(),
                    item,
                    itemRequest.Quantity,
                    itemRequest.IsDiscounted,
                    itemRequest.DiscountPercentage,
                    item.Price * itemRequest.Quantity);

                updatedItems.Add(newBasketItem);
            }
        }

        var updatedBasket = basket with { Items = updatedItems };
        return await basketsRepository.UpdateBasketAsync(updatedBasket, token);
    }
}
