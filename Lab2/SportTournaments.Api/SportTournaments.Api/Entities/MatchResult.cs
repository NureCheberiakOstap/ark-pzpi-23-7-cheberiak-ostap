namespace SportTournaments.Api.Entities;

public class MatchResult
{
    public Guid MatchId { get; set; }
    public Match Match { get; set; } = null!;

    public int HomeScore { get; set; }
    public int AwayScore { get; set; }

    public Guid? WinnerTeamId { get; set; }
    public Team? WinnerTeam { get; set; }

    public Guid EnteredByUserId { get; set; }
    public User EnteredByUser { get; set; } = null!;

    public DateTime EnteredAt { get; set; } = DateTime.UtcNow;
}
