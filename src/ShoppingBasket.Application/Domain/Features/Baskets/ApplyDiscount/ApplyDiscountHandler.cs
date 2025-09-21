using ShoppingBasket.Application.Components.Utils;
using ShoppingBasket.Application.Infrastructure.Repositories;
using ShoppingBasket.Application.Components.Services;

namespace ShoppingBasket.Application.Domain.Features.Baskets.ApplyDiscount;

using Result = DataResult<Models.Basket>;

public class ApplyDiscountHandler(
    IBasketsRepository basketsRepository) : IHandler<ApplyDiscountCommand, Result>
{
    public async Task<Result> ExecuteAsync(ApplyDiscountCommand request, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(request.DiscountCode))
            return Result.Failure(ErrorCodes.InvalidDiscountCode);

        var basketResult = await basketsRepository.GetBasketByIdAsync(request.BasketId, token);
        if (!basketResult.IsValid(out var basket))
            return Result.Failure(ErrorCodes.BasketNotFound);

        if (!DiscountCodeService.IsValidDiscountCode(request.DiscountCode))
            return Result.Failure(ErrorCodes.InvalidDiscountCode);

        basket.SetDiscountCode(request.DiscountCode);

        return await basketsRepository.UpdateBasketAsync(basket, token);
    }
}
