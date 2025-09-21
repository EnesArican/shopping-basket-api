using ShoppingBasket.Application.Components.Utils;
using ShoppingBasket.Application.Infrastructure.Repositories;
using ShoppingBasket.Application.Domain.Models;
using ShoppingBasket.Application.Components.Services;

namespace ShoppingBasket.Application.Domain.Features.Baskets.SetShipping;

using Result = DataResult<Models.Basket>;

public class SetShippingHandler(
    IBasketsRepository basketsRepository) : IHandler<SetShippingCommand, Result>
{
    private const decimal UkShippingCost = 5.99m;
    private const decimal InternationalShippingCost = 12.99m;

    public async Task<Result> ExecuteAsync(SetShippingCommand request, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(request.Country))
            return Result.Failure(ErrorCodes.InvalidShippingCountry);

        var basketResult = await basketsRepository.GetBasketByIdAsync(request.BasketId, token);
        if (!basketResult.IsValid(out var basket))
            return Result.Failure(ErrorCodes.BasketNotFound);

        var shippingCost = CalculateShippingCost(request.Country);
        basket.SetShipping(request.Country, shippingCost);

        return await basketsRepository.UpdateBasketAsync(basket, token);
    }

    private static decimal CalculateShippingCost(string country)
    {
        return string.Equals(country, "UK", StringComparison.OrdinalIgnoreCase) 
            ? UkShippingCost 
            : InternationalShippingCost;
    }
}
