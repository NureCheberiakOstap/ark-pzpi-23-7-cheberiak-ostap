namespace SportTournaments.Api.Dtos;

public class CreateRegistrationRequest
{
    public Guid TeamId { get; set; }
    public Guid ApplicantUserId { get; set; }
}
