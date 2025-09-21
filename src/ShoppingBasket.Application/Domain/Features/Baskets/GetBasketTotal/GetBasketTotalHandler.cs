using ShoppingBasket.Application.Components.Utils;
using ShoppingBasket.Application.Infrastructure.Repositories;
using ShoppingBasket.Application.Components.Services;

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

        var subTotal = CalculateSubTotal(basket.Items, basket.DiscountCode);
        var shippingCost = basket.ShippingCost ?? 0m;
        var totalBeforeVat = subTotal + shippingCost;
        var vatAmount = totalBeforeVat * VatRate;
        var totalWithVat = totalBeforeVat + vatAmount;
        var totalWithoutVat = totalBeforeVat;
        var totalItems = basket.Items.Sum(item => item.Quantity);

        var result = new BasketTotalResult(
            BasketId: basket.Id,
            SubTotal: subTotal,
            ShippingCost: shippingCost,
            VatAmount: vatAmount,
            TotalWithVat: totalWithVat,
            TotalWithoutVat: totalWithoutVat,
            TotalItems: totalItems);

        return Result.Success(result);
    }

    private static decimal CalculateSubTotal(List<Models.BasketItem> basketItems, string? basketDiscountCode)
    {
        // Calculate totals for already discounted items (no additional discount)
        var discountedItemsTotal = basketItems
            .Where(item => item.IsDiscounted)
            .Sum(item => item.TotalPrice);

        // Calculate totals for non-discounted items
        var nonDiscountedItemsTotal = basketItems
            .Where(item => !item.IsDiscounted)
            .Sum(item => item.TotalPrice);

        // Apply basket-level discount only to non-discounted items
        if (!string.IsNullOrWhiteSpace(basketDiscountCode) && 
            DiscountCodeService.TryGetDiscountPercentage(basketDiscountCode, out decimal discountPercentage))
        {
            var discountAmount = nonDiscountedItemsTotal * (discountPercentage / 100m);
            nonDiscountedItemsTotal -= discountAmount;
        }

        return discountedItemsTotal + nonDiscountedItemsTotal;
    }
}
