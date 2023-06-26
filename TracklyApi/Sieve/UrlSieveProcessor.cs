using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;
using TracklyApi.Entities;

namespace TracklyApi.Sieve;
public class UrlSieveProcessor : SieveProcessor
{
    public UrlSieveProcessor(IOptions<SieveOptions> options) : base(options) { }

    protected override SievePropertyMapper MapProperties(SievePropertyMapper mapper)
    {
        mapper.Property<ManagedUrl>(u => u.CreatedAt)
            .CanFilter()
            .CanSort();
        mapper.Property<ManagedUrl>(u => u.IsActive)
            .CanFilter()
            .CanSort();
        mapper.Property<ManagedUrl>(u => u.TotalClicks)
            .CanSort();
        mapper.Property<ManagedUrl>(u => u.NewPath)
            .CanFilter();
        mapper.Property<ManagedUrl>(u => u.TargetUrl)
            .CanFilter();
        mapper.Property<ManagedUrl>(u => u.TargetUrl)
            .CanFilter();
        mapper.Property<ManagedUrl>(u => u.User)
            .CanFilter();

        mapper.Property<UrlVisit>(u => u.VisitTimestamp)
            .CanFilter()
            .CanSort();
        mapper.Property<UrlVisit>(u => u.Url)
            .CanFilter();

        return mapper;
    }
}
