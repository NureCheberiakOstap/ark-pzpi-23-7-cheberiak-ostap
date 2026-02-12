using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportTournaments.Api.Data;
using SportTournaments.Api.Dtos;
using SportTournaments.Api.Entities;

namespace SportTournaments.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TournamentsController : BaseAuthController
{
    public TournamentsController(ApplicationDbContext db) : base(db) { }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTournamentRequest request)
    {
        var auth = await AuthorizeAsync("Organizer");
        if (auth != null) return auth;

        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest("Title is required.");

        var organizer = (User)HttpContext.Items["User"]!; // або CurrentUser

        var t = new Tournament
        {
            Title = request.Title.Trim(),
            SportType = request.SportType?.Trim() ?? "unknown",
            Format = request.Format?.Trim() ?? "single_elimination",
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            OrganizerUserId = organizer.Id,
            Status = "draft"
        };

        _db.Tournaments.Add(t);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = t.Id }, t);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? status = null)
    {
        var q = _db.Tournaments.AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            q = q.Where(x => x.Status == status);

        var list = await q
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var t = await _db.Tournaments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return t is null ? NotFound() : Ok(t);
    }

    [HttpPost("{id:guid}/publish")]
    public async Task<IActionResult> Publish(Guid id)
    {
        var auth = await AuthorizeAsync("Organizer");
        if (auth != null) return auth;

        var t = await _db.Tournaments.FirstOrDefaultAsync(x => x.Id == id);
        if (t is null) return NotFound();

        t.Status = "published";
        await _db.SaveChangesAsync();

        return Ok(t);
    }
}
