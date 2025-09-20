using ShoppingBasket.Application.Components.Utils;
using ShoppingBasket.Application.Infrastructure.Repositories;

namespace ShoppingBasket.Application.Domain.Features.Item.GetItems;

using Result = DataResult<List<Models.Item>>;

public class GetItemsHandler(
    IItemsRepository itemRepository) : IHandler<GetItemsQuery, Result>
{
    public async Task<Result> ExecuteAsync(GetItemsQuery request, CancellationToken token)
    {
        var response = await itemRepository.GetAllItemsAsync(token);

        if (!response.IsValid(out var items))
            return Result.Failure(response.ErrorCode);

        return Result.Success(items);
    }
}
