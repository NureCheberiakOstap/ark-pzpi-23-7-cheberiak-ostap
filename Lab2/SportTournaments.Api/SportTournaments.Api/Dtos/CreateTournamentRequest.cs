namespace SportTournaments.Api.Dtos;

public class CreateTournamentRequest
{
    public string Title { get; set; } = null!;
    public string SportType { get; set; } = "unknown";
    public string Format { get; set; } = "single_elimination";
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
