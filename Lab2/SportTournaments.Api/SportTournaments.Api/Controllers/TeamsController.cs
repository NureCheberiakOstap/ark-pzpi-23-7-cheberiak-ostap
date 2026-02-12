using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportTournaments.Api.Data;
using SportTournaments.Api.Dtos;
using SportTournaments.Api.Entities;

namespace SportTournaments.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamsController : BaseAuthController
{
    public TeamsController(ApplicationDbContext db) : base(db) { }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTeamRequest request)
    {
        var auth = await AuthorizeAsync("Participant", "Organizer");
        if (auth != null) return auth;

        var user = (User)HttpContext.Items["User"]!;

        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Team name is required.");

        var captainExists = await _db.Users.AnyAsync(u => u.Id == request.CaptainUserId);
        if (!captainExists) return BadRequest("CaptainUserId not found.");

        var team = new Team
        {
            Name = request.Name.Trim(),
            CaptainUserId = user.Id
        };

        _db.Teams.Add(team);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = team.Id }, team);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var team = await _db.Teams
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);

        return team is null ? NotFound() : Ok(team);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var teams = await _db.Teams
            .AsNoTracking()
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return Ok(teams);
    }
}
