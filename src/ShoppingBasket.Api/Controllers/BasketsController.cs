using Microsoft.AspNetCore.Mvc;
using ShoppingBasket.Api.Mappers;
using ShoppingBasket.Application.Domain.Features.Basket.CreateBasket;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppingBasket.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class BasketController(
    CreateBasketHandler createBasketHandler,
    ILogger<BasketController> logger) : ApiController
{
    /*
        POST /baskets → create new basket

        POST /baskets/{id}/items → Add item(s) to the basket (has isDiscounted property)

        DELETE /baskets/{id}/items/{itemId} → Remove an item from the basket

        POST /baskets/{id}/discount → apply discount code (excluding already discounted items)

        POST /baskets/{id}/shipping -> set shipping country (UK / other) and cost
    
        GET /baskets/total → Get total cost (2 params for "totalWithVat" & "totalWihtoutVat")
     */

    [HttpPost(ApiRoutes.Basket.Create)]
    [SwaggerOperation(Summary = "Create new basket")]
    public async Task<IActionResult> CreateBasket(CancellationToken cancellationToken = default)
    {
        var cmd = new CreateBasketCommand();
        var result = await createBasketHandler.ExecuteAsync(cmd, cancellationToken);

        if (!result.IsValid(out var basket))
            return BuildErrorResponse(result.ErrorCode);

        return StatusCode(StatusCodes.Status201Created, basket.ToDto());
    }


}
