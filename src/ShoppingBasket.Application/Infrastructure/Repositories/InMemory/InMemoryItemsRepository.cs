using Microsoft.Extensions.Logging;
using ShoppingBasket.Application.Components.Utils;
using ShoppingBasket.Application.Domain.Models;
using ShoppingBasket.Application.Infrastructure.Entities;
using ShoppingBasket.Application.Infrastructure.Mappers;

namespace ShoppingBasket.Application.Infrastructure.Repositories.InMemory;

public class InMemoryItemsRepository(
    ILogger<InMemoryItemsRepository> logger) : IItemsRepository
{
    private static readonly List<ItemEntity> HardcodedItems = 
    [
        new ItemEntity(
            new Guid("11111111-1111-1111-1111-111111111111"),
            "Laptop",
            1000.00m),
        new ItemEntity(
            new Guid("22222222-2222-2222-2222-222222222222"),
            "Wireless Mouse",
            30.00m),
        new ItemEntity(
            new Guid("33333333-3333-3333-3333-333333333333"),
            "Mechanical Keyboard",
            150.00m),
        new ItemEntity(
            new Guid("44444444-4444-4444-4444-444444444444"),
            "USB-C Hub",
            80.00m),
        new ItemEntity(
            new Guid("55555555-5555-5555-5555-555555555555"),
            "Wireless Headphones",
            200.00m),
        new ItemEntity(
            new Guid("66666666-6666-6666-6666-666666666666"),
            "External Monitor",
            300.00m),
        new ItemEntity(
            new Guid("77777777-7777-7777-7777-777777777777"),
            "Smartphone",
            700.00m),
        new ItemEntity(
            new Guid("88888888-8888-8888-8888-888888888888"),
            "Tablet",
            400.00m)
    ];

    public Task<DataResult<List<Item>>> GetAllItemsAsync(CancellationToken token)
    {
        try
        {
            logger.LogInformation("Retrieving all items from hardcoded catalog");
            var items = HardcodedItems.Select(entity => entity.ToDomain()).ToList();
            return Task.FromResult(DataResult<List<Item>>.Success(items));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve items");
            return Task.FromResult(DataResult<List<Item>>.Failure(ErrorCodes.ServerError));
        }
    }

    public Task<DataResult<Item>> GetItemByIdAsync(Guid id, CancellationToken token)
    {
        try
        {
            var itemEntity = HardcodedItems.FirstOrDefault(x => x.Id == id);
            if (itemEntity == null)
            {
                logger.LogWarning("Item with ID {ItemId} not found", id);
                return Task.FromResult(DataResult<Item>.Failure(ErrorCodes.NotFound));
            }

            logger.LogInformation("Retrieved item {ItemId}: {ItemName}", id, itemEntity.Name);
            var item = itemEntity.ToDomain();
            return Task.FromResult(DataResult<Item>.Success(item));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve item with ID {ItemId}", id);
            return Task.FromResult(DataResult<Item>.Failure(ErrorCodes.ServerError));
        }
    }
}
