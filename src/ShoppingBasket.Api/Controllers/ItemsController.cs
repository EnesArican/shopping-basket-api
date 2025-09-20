using Microsoft.AspNetCore.Mvc;
using ShoppingBasket.Api.Mappers;
using ShoppingBasket.Application.Domain.Features.Item.GetItems;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppingBasket.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemController(
    GetItemsHandler getItemsHandler,
    ILogger<ItemController> logger) : ApiController
{
    [HttpGet(ApiRoutes.Item.GetAll)]
    [SwaggerOperation(Summary = "Get all available items")]
    public async Task<IActionResult> GetItems(CancellationToken cancellationToken = default)
    {
        var query = new GetItemsQuery();
        var result = await getItemsHandler.ExecuteAsync(query, cancellationToken);

        if (!result.IsValid(out var items))
            return BuildErrorResponse(result.ErrorCode);

        var itemDtos = items.Select(item => item.ToDto()).ToList();
        return Ok(itemDtos);
    }
}
