using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ShoppingBasket.Api.Dtos;
using ShoppingBasket.Application.Components.Utils;

namespace ShoppingBasket.Api.Controllers.Filters;

public class CustomExceptionFilterAttribute(
    ILogger<CustomExceptionFilterAttribute> logger) : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        logger.LogError(context.Exception, "Unexpected exception caught in exception filter");
        var rsp = new ErrorResponseDto(ErrorCodes.ServerError);
        context.Result = new ObjectResult(rsp)
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };

    }
}