using ShoppingBasket.Application.Components.Utils;
using ShoppingBasket.Application.Infrastructure.Repositories;

namespace ShoppingBasket.Application.Domain.Features.Basket.CreateBasket;

using Result = DataResult<Models.Basket>;

public class CreateBasketHandler(
    IBasketsRepository basketsRepository) : IHandler<CreateBasketCommand, Result>
{
    public async Task<Result> ExecuteAsync(CreateBasketCommand request, CancellationToken token)
    {
        return await basketsRepository.CreateBasketAsync(token);
    }
}
