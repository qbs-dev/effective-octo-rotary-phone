using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TracklyApi.Entities;

public partial class UrlVisit
{
    public long Url { get; set; }

    public DateTime VisitTimestamp { get; set; }

    public IPAddress IpAddress { get; set; } = null!;

    public string CountryCode { get; set; } = null!;

    public string? BrowserFingerprint { get; set; }

    public virtual ManagedUrl UrlNavigation { get; set; } = null!;
}

public class UrlVisitTypeConfiguration : IEntityTypeConfiguration<UrlVisit>
{
    public void Configure(EntityTypeBuilder<UrlVisit> builder)
    {
        builder.HasKey(e => new { e.Url, e.VisitTimestamp });

        builder.ToTable("url_visits");

        builder.HasIndex(e => e.CountryCode, "IXFK_url_visits_countries");

        builder.HasIndex(e => e.Url, "IXFK_url_visits_managed_urls");

        builder.Property(e => e.Url);
        builder.Property(e => e.VisitTimestamp);
        builder.Property(e => e.BrowserFingerprint)
            .HasMaxLength(128);
        builder.Property(e => e.CountryCode)
            .HasMaxLength(2);
        builder.Property(e => e.IpAddress);

        builder.HasOne(d => d.UrlNavigation).WithMany(p => p.UrlVisits)
            .HasForeignKey(d => d.Url)
            .HasConstraintName("FK_url_visits_managed_urls")
            .OnDelete(DeleteBehavior.Cascade);
    }
}

