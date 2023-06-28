using Microsoft.EntityFrameworkCore;
using EFCore.NamingConventions;

namespace TracklyApi.Entities;

public class TracklyDbContext : DbContext
{
    public TracklyDbContext() { }

    public TracklyDbContext(DbContextOptions<TracklyDbContext> options)
        : base(options) { }

    public virtual DbSet<EmailConfirmation> EmailConfirmations { get; set; } = null!;

    public virtual DbSet<ManagedUrl> ManagedUrls { get; set; } = null!;

    public virtual DbSet<RefreshSession> RefreshSessions { get; set; } = null!;

    public virtual DbSet<UrlVisit> UrlVisits { get; set; } = null!;

    public virtual DbSet<User> Users { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new UserTypeConfiguration().Configure(modelBuilder.Entity<User>());
        new EmailConfirmationTypeConfiguration().Configure(modelBuilder.Entity<EmailConfirmation>());
        new ManagedUrlTypeConfiguration().Configure(modelBuilder.Entity<ManagedUrl>());
        new RefreshSessionTypeConfiguration().Configure(modelBuilder.Entity<RefreshSession>());
        new UrlVisitTypeConfiguration().Configure(modelBuilder.Entity<UrlVisit>());
    }
}
