using ShoppingBasket.Application.Components.Utils;
using ShoppingBasket.Application.Infrastructure.Repositories;

namespace ShoppingBasket.Application.Domain.Features.Item.GetItems;

using Result = DataResult<List<Models.Item>>;

public class GetItemsHandler(
    IItemsRepository itemsRepository) : IHandler<GetItemsQuery, Result>
{
    public async Task<Result> ExecuteAsync(GetItemsQuery request, CancellationToken token)
    {
        return await itemsRepository.GetAllItemsAsync(token);
    }
}
