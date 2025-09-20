using ShoppingBasket.Application.Components.Utils;
using ShoppingBasket.Application.Infrastructure.Repositories;

namespace ShoppingBasket.Application.Domain.Features.Baskets.GetBasketTotal;

using Result = DataResult<BasketTotalResult>;

public class GetBasketTotalHandler(
    IBasketsRepository basketsRepository) : IHandler<GetBasketTotalQuery, Result>
{
    private const decimal VatRate = 0.20m; // 20% VAT

    public async Task<Result> ExecuteAsync(GetBasketTotalQuery request, CancellationToken token)
    {
        var basketResult = await basketsRepository.GetBasketByIdAsync(request.BasketId, token);

        if (!basketResult.IsValid(out var basket))
            return Result.Failure(basketResult.ErrorCode);

        var subTotal = CalculateSubTotal(basket.Items);
        var vatAmount = subTotal * VatRate;
        var totalWithVat = subTotal + vatAmount;
        var totalWithoutVat = subTotal;
        var totalItems = basket.Items.Sum(item => item.Quantity);

        var result = new BasketTotalResult(
            BasketId: basket.Id,
            SubTotal: subTotal,
            VatAmount: vatAmount,
            TotalWithVat: totalWithVat,
            TotalWithoutVat: totalWithoutVat,
            TotalItems: totalItems);

        return Result.Success(result);
    }

    private static decimal CalculateSubTotal(List<Models.BasketItem> basketItems)
    {
        return basketItems.Sum(item => item.TotalPrice);
    }
}
