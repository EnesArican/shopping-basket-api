using ShoppingBasket.Application.Components.Utils;
using ShoppingBasket.Application.Infrastructure.Repositories;

namespace ShoppingBasket.Application.Domain.Features.Basket.AddBasketItem;

using Result = DataResult<bool>;

public class AddBasketItemHandler(
    IBasketRepository basketRepository) : IHandler<AddBasketItemCommand, Result>
{
    public async Task<Result> ExecuteAsync(AddBasketItemCommand request, CancellationToken token)
    {
        //TODO: Add implementation
        return Result.Success(true);
    }
}
