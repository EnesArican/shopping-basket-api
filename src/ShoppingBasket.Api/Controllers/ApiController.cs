using Microsoft.AspNetCore.Mvc;
using ShoppingBasket.Api.Dtos;
using ShoppingBasket.Application.Components.Utils;

namespace ShoppingBasket.Api.Controllers;

public abstract class ApiController : ControllerBase
{
    protected IActionResult BuildErrorResponse(string? errorCode)
    {
        var rsp = new ErrorResponseDto(errorCode ?? ErrorCodes.ServerError);
        return errorCode switch
        {
            ErrorCodes.NotFound or 
            ErrorCodes.ItemNotFound or 
            ErrorCodes.BasketNotFound => NotFound(rsp),
            ErrorCodes.InvalidQuantity or
            ErrorCodes.InvalidDiscountPercentage or
            ErrorCodes.InvalidDiscountCode or
            ErrorCodes.InvalidRequest => UnprocessableEntity(rsp),
            ErrorCodes.ServerError or _ => ServerError(rsp),
        };
    }

    private static ObjectResult ServerError(ErrorResponseDto rsp) =>
       new(rsp) { StatusCode = StatusCodes.Status500InternalServerError };
}
