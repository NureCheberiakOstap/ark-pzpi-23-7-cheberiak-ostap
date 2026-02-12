using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportTournaments.Api.Data;
using SportTournaments.Api.Dtos;
using SportTournaments.Api.Entities;

namespace SportTournaments.Api.Controllers;

[ApiController]
[Route("api")]
public class RegistrationsController : BaseAuthController
{
    public RegistrationsController(ApplicationDbContext db) : base(db) { }

    // Подати заявку на турнір
    // POST /api/tournaments/{tournamentId}/registrations
    [HttpPost("tournaments/{tournamentId:guid}/registrations")]
    public async Task<IActionResult> Create(Guid tournamentId, CreateRegistrationRequest request)
    {
        var tournament = await _db.Tournaments.FirstOrDefaultAsync(t => t.Id == tournamentId);
        if (tournament is null) return NotFound("Tournament not found.");

        // (опціонально) не дозволяти заявки, якщо турнір не draft/published
        if (tournament.Status == "closed")
            return BadRequest("Tournament is closed.");

        var teamExists = await _db.Teams.AnyAsync(t => t.Id == request.TeamId);
        if (!teamExists) return BadRequest("TeamId not found.");

        var userExists = await _db.Users.AnyAsync(u => u.Id == request.ApplicantUserId);
        if (!userExists) return BadRequest("ApplicantUserId not found.");

        // Унікальність (TournamentId, TeamId) вже забезпечена індексом.
        var alreadyExists = await _db.TournamentRegistrations
            .AnyAsync(r => r.TournamentId == tournamentId && r.TeamId == request.TeamId);

        if (alreadyExists) return Conflict("This team is already registered for the tournament.");

        var reg = new TournamentRegistration
        {
            TournamentId = tournamentId,
            TeamId = request.TeamId,
            ApplicantUserId = request.ApplicantUserId,
            Status = "pending"
        };

        _db.TournamentRegistrations.Add(reg);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = reg.Id }, reg);
    }

    // GET /api/registrations/{id}
    [HttpGet("registrations/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var reg = await _db.TournamentRegistrations
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);

        return reg is null ? NotFound() : Ok(reg);
    }

    // Список заявок турніру
    // GET /api/tournaments/{tournamentId}/registrations
    [HttpGet("tournaments/{tournamentId:guid}/registrations")]
    public async Task<IActionResult> GetTournamentRegistrations(Guid tournamentId)
    {
        var exists = await _db.Tournaments.AnyAsync(t => t.Id == tournamentId);
        if (!exists) return NotFound("Tournament not found.");

        var regs = await _db.TournamentRegistrations
            .AsNoTracking()
            .Where(r => r.TournamentId == tournamentId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return Ok(regs);
    }

    // Approve
    // POST /api/registrations/{id}/approve
    [HttpPost("registrations/{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id)
    {
        var auth = await AuthorizeAsync("Organizer");
        if (auth != null) return auth;

        var reg = await _db.TournamentRegistrations.FirstOrDefaultAsync(r => r.Id == id);
        if (reg is null) return NotFound();

        if (reg.Status != "pending")
            return BadRequest("Only pending registrations can be approved.");

        reg.Status = "approved";
        await _db.SaveChangesAsync();

        return Ok(reg);
    }

    // Reject
    // POST /api/registrations/{id}/reject
    [HttpPost("registrations/{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id)
    {
        var auth = await AuthorizeAsync("Organizer");
        if (auth != null) return auth;

        var reg = await _db.TournamentRegistrations.FirstOrDefaultAsync(r => r.Id == id);
        if (reg is null) return NotFound();

        if (reg.Status != "pending")
            return BadRequest("Only pending registrations can be rejected.");

        reg.Status = "rejected";
        await _db.SaveChangesAsync();

        return Ok(reg);
    }
}
