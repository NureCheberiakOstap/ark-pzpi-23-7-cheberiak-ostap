using Microsoft.EntityFrameworkCore;
using SportTournaments.Api.Entities;

namespace SportTournaments.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Tournament> Tournaments => Set<Tournament>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<TournamentRegistration> TournamentRegistrations => Set<TournamentRegistration>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<MatchResult> MatchResults => Set<MatchResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Role>()
            .HasIndex(r => r.Name)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Tournament>()
            .HasOne(t => t.OrganizerUser)
            .WithMany()
            .HasForeignKey(t => t.OrganizerUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Team>()
            .HasOne(t => t.CaptainUser)
            .WithMany()
            .HasForeignKey(t => t.CaptainUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TournamentRegistration>()
            .HasIndex(r => new { r.TournamentId, r.TeamId })
            .IsUnique();

        modelBuilder.Entity<TournamentRegistration>()
            .HasOne(r => r.ApplicantUser)
            .WithMany()
            .HasForeignKey(r => r.ApplicantUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Match>()
            .HasOne(m => m.HomeTeam)
            .WithMany()
            .HasForeignKey(m => m.HomeTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Match>()
            .HasOne(m => m.AwayTeam)
            .WithMany()
            .HasForeignKey(m => m.AwayTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MatchResult>()
            .HasKey(r => r.MatchId);

        modelBuilder.Entity<MatchResult>()
            .HasOne(r => r.Match)
            .WithOne(m => m.Result)
            .HasForeignKey<MatchResult>(r => r.MatchId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MatchResult>()
            .HasOne(r => r.EnteredByUser)
            .WithMany()
            .HasForeignKey(r => r.EnteredByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
