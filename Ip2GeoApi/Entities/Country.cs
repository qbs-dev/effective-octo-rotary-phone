using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NpgsqlTypes;

namespace Ip2GeoApi.Entities;
public class Country
{
    public int Id { get; set; }
    public NpgsqlRange<IPAddress> IpRange { get; set; }
    public string CountryCode { get; set; } = null!;
}

public class IpRangeTypeConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id);
        builder.Property(e => e.IpRange)
            .HasColumnType("inetrange");
        builder.Property(e => e.CountryCode)
            .HasMaxLength(2);
    }
}
