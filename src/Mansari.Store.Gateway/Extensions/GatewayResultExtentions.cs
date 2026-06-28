using Mansari.Store.Gateway.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace Mansari.Store.Gateway.Extensions;

public static class GatewayResultExtensions
{
    public static IActionResult ToActionResult<T>(
        this GatewayResult<T> result)
    {
        if (result.IsSuccess)
        {
            return new OkObjectResult(result.Value);
        }

        return result.Error!.Code switch
        {
            "Validation.Error"
                => new BadRequestObjectResult(result.Error),

            "Basket.NotFound"
                => new NotFoundObjectResult(result.Error),

            "Basket.Unavailable"
                => new ObjectResult(result.Error)
                {
                    StatusCode = StatusCodes.Status503ServiceUnavailable
                },

            _ => new ObjectResult(result.Error)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            }
        };
    }
}