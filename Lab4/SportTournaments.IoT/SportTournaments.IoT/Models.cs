public sealed class LoginRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

public sealed class AuthResponse
{
    public string UserId { get; set; } = "";
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Role { get; set; } = "";
    public string Token { get; set; } = "";
}

public sealed class TournamentListItem
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Status { get; set; } = "";
}

public sealed class MatchListItem
{
    public string Id { get; set; } = "";
    public int Round { get; set; }
    public string HomeTeamId { get; set; } = "";
    public string AwayTeamId { get; set; } = "";
    public string Status { get; set; } = "";
    public string Location { get; set; } = "";
    public string ScheduledAt { get; set; } = "";
}

public sealed class CreateMatchResultRequest
{
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
}

public sealed class StandingsRow
{
    public string TeamId { get; set; } = "";
    public string TeamName { get; set; } = "";
    public int Played { get; set; }
    public int Wins { get; set; }
    public int Draws { get; set; }
    public int Losses { get; set; }
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public int GoalDiff { get; set; }
    public int Points { get; set; }
}
