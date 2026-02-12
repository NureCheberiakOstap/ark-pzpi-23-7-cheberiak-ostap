namespace SportTournaments.Api.Dtos;

public class AuthResponse
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string Token { get; set; } = null!; // простий токен (GUID), не JWT
}
