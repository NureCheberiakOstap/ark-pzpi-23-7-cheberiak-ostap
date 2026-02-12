namespace SportTournaments.Api.Dtos;

public class CreateTeamRequest
{
    public string Name { get; set; } = null!;
    public Guid CaptainUserId { get; set; }
}
