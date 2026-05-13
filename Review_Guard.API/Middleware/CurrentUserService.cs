using Review_Guard.Application.Abstractions.Services.CurrentUserService;
using System.Security.Claims;

namespace Review_Guard.API.Middleware;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var value = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return value is not null && Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public bool IsAdmin => User?.IsInRole("Admin") == true || User?.IsInRole("SuperAdmin") == true;

    public string? IpAddress =>
        _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString()
        ?? _httpContextAccessor.HttpContext?.Request.Headers["X-Forwarded-For"].FirstOrDefault();

    public string? UserAgent =>
        _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].FirstOrDefault();
}
