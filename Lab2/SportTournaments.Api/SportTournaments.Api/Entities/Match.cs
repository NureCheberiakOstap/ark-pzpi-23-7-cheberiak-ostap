namespace SportTournaments.Api.Entities;

public class Match
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TournamentId { get; set; }
    public Tournament Tournament { get; set; } = null!;

    public int Round { get; set; }

    public Guid HomeTeamId { get; set; }
    public Team HomeTeam { get; set; } = null!;

    public Guid AwayTeamId { get; set; }
    public Team AwayTeam { get; set; } = null!;

    public DateTime ScheduledAt { get; set; }
    public string? Location { get; set; }

    public string Status { get; set; } = "scheduled"; // scheduled/finished/canceled

    public MatchResult? Result { get; set; }
}
