namespace SportTournaments.Api.Entities;

public class TournamentRegistration
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TournamentId { get; set; }
    public Tournament Tournament { get; set; } = null!;

    public Guid TeamId { get; set; }
    public Team Team { get; set; } = null!;

    public Guid ApplicantUserId { get; set; }
    public User ApplicantUser { get; set; } = null!;

    public string Status { get; set; } = "pending"; // pending/approved/rejected
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
