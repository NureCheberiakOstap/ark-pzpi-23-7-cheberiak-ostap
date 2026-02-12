using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportTournaments.Api.Data;
using SportTournaments.Api.Dtos;
using SportTournaments.Api.Entities;
using SportTournaments.Api.Helpers;
using SportTournaments.Api.Services;

namespace SportTournaments.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private static readonly Dictionary<string, Guid> _tokens = new();
    // навчальний “in-memory” token store: token -> userId

    private readonly ApplicationDbContext _db;
    public AuthController(ApplicationDbContext db) => _db = db;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Name, Email, Password are required.");
        }

        var email = request.Email.Trim().ToLowerInvariant();

        var exists = await _db.Users.AnyAsync(u => u.Email.ToLower() == email);
        if (exists) return Conflict("User with this email already exists.");

        var roleName = string.IsNullOrWhiteSpace(request.Role) ? "Participant" : request.Role.Trim();
        var role = await _db.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        if (role is null) return BadRequest("Role not found. Use: Admin/Organizer/Judge/Participant/Viewer.");

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = email,
            PasswordHash = PasswordHasher.Hash(request.Password),
            RoleId = role.Id,
            IsActive = true
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            user.Id,
            user.Name,
            user.Email,
            Role = role.Name
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Email and Password are required.");

        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _db.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email);

        if (user is null) return Unauthorized("Invalid email or password.");
        if (!user.IsActive) return Unauthorized("User is inactive.");

        var ok = PasswordHasher.Verify(request.Password, user.PasswordHash);
        if (!ok) return Unauthorized("Invalid email or password.");

        var token = Guid.NewGuid().ToString("N");
        AuthTokenService.Add(token, user.Id); 

        var response = new AuthResponse
        {
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.Name,
            Token = token
        };

        return Ok(response);
    }

    [HttpPost("logout")]
    public IActionResult Logout([FromHeader(Name = "X-Auth-Token")] string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return BadRequest("X-Auth-Token header is required.");

        _tokens.Remove(token);
        return Ok("Logged out.");
    }

    // Допоміжний endpoint: отримати поточного користувача по токену
    [HttpGet("me")]
    public async Task<IActionResult> Me([FromHeader(Name = "X-Auth-Token")] string? token)
    {
        if (string.IsNullOrWhiteSpace(token)) return Unauthorized("X-Auth-Token header is required.");

        if (!_tokens.TryGetValue(token, out var userId))
            return Unauthorized("Invalid token.");

        var user = await _db.Users
            .Include(u => u.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null) return Unauthorized("User not found.");

        return Ok(new
        {
            user.Id,
            user.Name,
            user.Email,
            Role = user.Role.Name
        });
    }
}
