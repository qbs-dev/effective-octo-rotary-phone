using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TracklyApi.Entities;

public partial class ManagedUrl
{
    public long Id { get; set; }

    public int User { get; set; }

    public bool IsActive { get; set; }

    public string NewPath { get; set; } = null!;

    public string TargetUrl { get; set; } = null!;

    public string Description { get; set; } = null!;

    public long TotalClicks { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<UrlVisit> UrlVisits { get; set; } = new List<UrlVisit>();

    public virtual User UserNavigation { get; set; } = null!;
}

public class ManagedUrlTypeConfiguration : IEntityTypeConfiguration<ManagedUrl>
{
    public void Configure(EntityTypeBuilder<ManagedUrl> builder)
    {
        builder.HasKey(e => e.Id);

        builder.ToTable("managed_urls");

        builder.HasIndex(e => e.User, "IXFK_managed_urls_users");

        builder.HasIndex(e => e.NewPath, "UNQ_new_path").IsUnique();

        builder.Property(e => e.Id)
            .UseIdentityAlwaysColumn();
        builder.Property(e => e.CreatedAt);
        builder.Property(e => e.Description)
            .HasMaxLength(256);
        builder.Property(e => e.IsActive);
        builder.Property(e => e.NewPath)
            .HasMaxLength(64);
        builder.Property(e => e.TargetUrl)
            .HasMaxLength(128);
        builder.Property(e => e.TotalClicks);
        builder.Property(e => e.User);

        builder.HasOne(d => d.UserNavigation).WithMany(p => p.ManagedUrls)
            .HasForeignKey(d => d.User)
            .HasConstraintName("FK_managed_urls_users")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
