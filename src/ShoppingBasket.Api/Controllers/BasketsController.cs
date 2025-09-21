using Microsoft.AspNetCore.Mvc;
using ShoppingBasket.Api.Dtos;
using ShoppingBasket.Api.Mappers;
using ShoppingBasket.Application.Domain.Features.Baskets.AddBasketItems;
using ShoppingBasket.Application.Domain.Features.Baskets.ApplyDiscount;
using ShoppingBasket.Application.Domain.Features.Baskets.CreateBasket;
using ShoppingBasket.Application.Domain.Features.Baskets.GetBasketTotal;
using ShoppingBasket.Application.Domain.Features.Baskets.RemoveBasketItem;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppingBasket.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class BasketsController(
    CreateBasketHandler createBasketHandler,
    AddBasketItemsHandler addBasketItemsHandler,
    RemoveBasketItemHandler removeBasketItemHandler,
    GetBasketTotalHandler getBasketTotalHandler,
    ApplyDiscountHandler applyDiscountHandler,
    ILogger<BasketsController> logger) : ApiController
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

    [HttpPost("{id}/" + ApiRoutes.Basket.AddItem)]
    [SwaggerOperation(Summary = "Add items to basket")]
    public async Task<IActionResult> AddItemsToBasket(
        Guid id, 
        [FromBody] AddBasketItemsRequestDto request, 
        CancellationToken cancellationToken = default)
    {
        var basketItemRequests = request.Items.Select(item => new BasketItemRequest(
            item.ItemId,
            item.Quantity,
            item.IsDiscounted,
            item.DiscountPercentage)).ToList();

        var cmd = new AddBasketItemsCommand(id, basketItemRequests);

        var result = await addBasketItemsHandler.ExecuteAsync(cmd, cancellationToken);

        if (!result.IsValid(out var basket))
            return BuildErrorResponse(result.ErrorCode);

        return Ok(basket.ToDto());
    }

    [HttpDelete("{id}/" + ApiRoutes.Basket.RemoveItem)]
    [SwaggerOperation(Summary = "Remove item from basket")]
    public async Task<IActionResult> RemoveItemFromBasket(
        Guid id,
        Guid itemId,
        CancellationToken cancellationToken = default)
    {
        var cmd = new RemoveBasketItemCommand(id, itemId);
        var result = await removeBasketItemHandler.ExecuteAsync(cmd, cancellationToken);

        if (!result.IsValid(out var basket))
            return BuildErrorResponse(result.ErrorCode);

        return Ok(basket.ToDto());
    }

    [HttpGet(ApiRoutes.Basket.GetTotal)]
    [SwaggerOperation(Summary = "Get basket total with and without VAT")]
    public async Task<IActionResult> GetBasketTotal(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetBasketTotalQuery(id);
        var result = await getBasketTotalHandler.ExecuteAsync(query, cancellationToken);

        if (!result.IsValid(out var basketTotal))
            return BuildErrorResponse(result.ErrorCode);

        return Ok(basketTotal.ToDto());
    }

    [HttpPost("{id}/" + ApiRoutes.Basket.ApplyDiscount)]
    [SwaggerOperation(Summary = "Apply discount code to basket")]
    public async Task<IActionResult> ApplyDiscountToBasket(
        Guid id,
        [FromBody] ApplyDiscountRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var cmd = new ApplyDiscountCommand(id, request.DiscountCode);
        var result = await applyDiscountHandler.ExecuteAsync(cmd, cancellationToken);

        if (!result.IsValid(out var basket))
            return BuildErrorResponse(result.ErrorCode);

        return Ok(basket.ToDto());
    }
}
