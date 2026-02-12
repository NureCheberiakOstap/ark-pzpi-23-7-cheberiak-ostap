using System.ComponentModel.DataAnnotations;

namespace SportTournaments.Api.Entities;

public class Team
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(200)]
    public string Name { get; set; } = null!;

    public Guid CaptainUserId { get; set; }
    public User CaptainUser { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
