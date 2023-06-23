using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TracklyApi.Entities;

public partial class RefreshSession
{
    public Guid RefreshToken { get; set; }

    public int User { get; set; }

    public string Fingerprint { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresIn { get; set; }

    public virtual User UserNavigation { get; set; } = null!;
}

public class RefreshSessionTypeConfiguration : IEntityTypeConfiguration<RefreshSession>
{
    public void Configure(EntityTypeBuilder<RefreshSession> builder)
    {
        builder.HasKey(e => e.RefreshToken);

        builder.ToTable("refresh_sessions");

        builder.HasIndex(e => e.RefreshToken, "INDX_refresh_session");

        builder.HasIndex(e => e.User, "IXFK_refresh_sessions_users");

        builder.Property(e => e.CreatedAt);
        builder.Property(e => e.ExpiresIn);
        builder.Property(e => e.Fingerprint)
            .HasMaxLength(128);
        builder.Property(e => e.RefreshToken);
        builder.Property(e => e.User);

        builder.HasOne(d => d.UserNavigation).WithMany(p => p.RefreshSessions)
            .HasForeignKey(d => d.User)
            .HasConstraintName("FK_refresh_sessions_users")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
