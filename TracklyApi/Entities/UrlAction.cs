using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TracklyApi.Entities;

public enum ActionTypes
{
    GetBrowserFingerprint = 0,
    GetCountryByIp,
    ForbidAccessByIpRange,
}

public partial class UrlAction
{
    public long Id { get; set; }

    public long Url { get; set; }

    public ActionTypes ActionType { get; set; }

    public string Value { get; set; } = null!;

    public virtual ManagedUrl UrlNavigation { get; set; } = null!;
}

public class UrlActionTypeConfiguration : IEntityTypeConfiguration<UrlAction>
{
    public void Configure(EntityTypeBuilder<UrlAction> builder)
    {
        builder.HasKey(e => e.Id);

        builder.ToTable("url_actions");

        builder.HasIndex(e => e.Url, "IXFK_url_actions_managed_urls");

        builder.Property(e => e.Id)
            .UseIdentityAlwaysColumn();
        builder.Property(e => e.ActionType);
        builder.Property(e => e.Url);
        builder.Property(e => e.Value)
            .HasMaxLength(128);

        builder.HasOne(d => d.UrlNavigation).WithMany(p => p.UrlActions)
            .HasForeignKey(d => d.Url)
            .HasConstraintName("FK_url_actions_managed_urls")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
