using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SportTournaments.Api.Entities;

public class Tournament
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(200)]
    public string Title { get; set; } = null!;

    [MaxLength(50)]
    public string SportType { get; set; } = "unknown";

    [MaxLength(50)]
    public string Format { get; set; } = "single_elimination"; // text

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    [MaxLength(30)]
    public string Status { get; set; } = "draft"; // draft/published/closed

    public Guid OrganizerUserId { get; set; }
    public User OrganizerUser { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<TournamentRegistration> Registrations { get; set; } = new List<TournamentRegistration>();
    public ICollection<Match> Matches { get; set; } = new List<Match>();
}
