using ShoppingBasket.Application.Components.Utils;
using ShoppingBasket.Application.Domain.Models;

namespace ShoppingBasket.Application.Infrastructure.Repositories;

public interface IItemsRepository
{
    Task<DataResult<List<Item>>> GetAllItemsAsync(CancellationToken token);
    Task<DataResult<Item>> GetItemByIdAsync(Guid id, CancellationToken token);
}
