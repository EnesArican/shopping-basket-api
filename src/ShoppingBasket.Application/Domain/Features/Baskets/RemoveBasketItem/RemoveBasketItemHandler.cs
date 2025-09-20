using ShoppingBasket.Application.Components.Utils;
using ShoppingBasket.Application.Infrastructure.Repositories;

namespace ShoppingBasket.Application.Domain.Features.Baskets.RemoveBasketItem;

using Result = DataResult<Models.Basket>;

public class RemoveBasketItemHandler(
    IBasketsRepository basketsRepository) : IHandler<RemoveBasketItemCommand, Result>
{
    public async Task<Result> ExecuteAsync(RemoveBasketItemCommand request, CancellationToken token)
    {
        var basketResult = await basketsRepository.GetBasketByIdAsync(request.BasketId, token);
        if (!basketResult.IsValid(out var basket))
            return Result.Failure(ErrorCodes.BasketNotFound);

        var existingItem = basket!.Items.FirstOrDefault(i => i.Item.Id == request.ItemId);
        if (existingItem == null)
            return Result.Failure(ErrorCodes.ItemNotFound);

        var updatedItems = basket.Items.Where(i => i.Item.Id != request.ItemId).ToList();
        var updatedBasket = basket with { Items = updatedItems };

        return await basketsRepository.UpdateBasketAsync(updatedBasket, token);
    }
}
