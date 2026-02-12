using System.ComponentModel.DataAnnotations;

namespace SportTournaments.Api.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(120)]
    public string Name { get; set; } = null!;

    [MaxLength(200)]
    public string Email { get; set; } = null!;

    [MaxLength(300)]
    public string PasswordHash { get; set; } = null!;

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}
