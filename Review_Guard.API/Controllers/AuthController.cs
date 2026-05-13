using MediatR;
using Microsoft.AspNetCore.Mvc;
using Review_Guard.Application.Feature.Auth.Command.Registration;
using Review_Guard.Application.Feature.Auth.DTOs.Requests;

namespace Review_Guard.API.Controllers;

public class AuthController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;
    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto request, CancellationToken ct)
    {
        var result = await _mediator.Send(new RegistrationRegisterUserCommand(request), ct);

        return HandleResult(result);
    }
}
