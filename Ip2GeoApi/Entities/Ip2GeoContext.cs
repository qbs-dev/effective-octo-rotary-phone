using Microsoft.EntityFrameworkCore;

namespace Ip2GeoApi.Entities;
public class Ip2GeoContext : DbContext
{
    public Ip2GeoContext() { }

    public Ip2GeoContext(DbContextOptions<Ip2GeoContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public virtual DbSet<Country> Countries { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresRange("inetrange", "inet");
        new IpRangeTypeConfiguration().Configure(modelBuilder.Entity<Country>());
    }
}
