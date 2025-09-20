using ShoppingBasket.Application.Components.Utils;
using ShoppingBasket.Application.Infrastructure.Repositories;

namespace ShoppingBasket.Application.Domain.Features.Basket.CreateBasket;

using Result = DataResult<Models.Basket>;

public class CreateBasketHandler(
    IBasketsRepository basketRepository) : IHandler<CreateBasketCommand, Result>
{
    public async Task<Result> ExecuteAsync(CreateBasketCommand request, CancellationToken token)
    {
        var response = await basketRepository.CreateBasketAsync(token);

        if (!response.IsValid(out var basket))
            return Result.Failure(response.ErrorCode);

        return Result.Success(basket);
    }
}
