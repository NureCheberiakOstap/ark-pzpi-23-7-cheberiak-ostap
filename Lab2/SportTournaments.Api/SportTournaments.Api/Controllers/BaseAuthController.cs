using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportTournaments.Api.Data;
using SportTournaments.Api.Services;

namespace SportTournaments.Api.Controllers;

public abstract class BaseAuthController : ControllerBase
{
    protected readonly ApplicationDbContext _db;

    protected BaseAuthController(ApplicationDbContext db)
    {
        _db = db;
    }

    protected async Task<IActionResult?> AuthorizeAsync(params string[] roles)
    {
        if (!Request.Headers.TryGetValue("X-Auth-Token", out var token))
            return Unauthorized("X-Auth-Token header is required.");

        if (!AuthTokenService.TryGetUserId(token!, out var userId))
            return Unauthorized("Invalid token.");

        var user = await _db.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null || !user.IsActive)
            return Unauthorized("User not found or inactive.");

        if (roles.Length > 0 && !roles.Contains(user.Role.Name))
            return Forbid($"Required role: {string.Join(", ", roles)}");

        HttpContext.Items["User"] = user;
        return null;
    }
}
