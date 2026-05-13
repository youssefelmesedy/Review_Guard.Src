using Microsoft.AspNetCore.Mvc;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.Auth;

namespace Review_Guard.API.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return HandleError(result);
    }

    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return HandleError(result);
    }

    private IActionResult HandleError(Result result)
    {
        var response = new
        {
            success = false,
            errorCode = result.ErrorCode,
            message = result.ErrorMessage
        };

        return result.ErrorCode switch
        {
            // Auth
            AuthMessage.UserAlreadyExists => Conflict(response),

            AuthMessage.InvalidCredentials => Unauthorized(response),

            AuthMessage.EmailNotVerified => Unauthorized(response),

            AuthMessage.RegisterationFailed => BadRequest(response),

            // Common
            "ValidationError" => BadRequest(response),

            "NotFound" => NotFound(response),

            "Forbidden" => StatusCode(403, response),

            _ => BadRequest(response)
        };
    }
}