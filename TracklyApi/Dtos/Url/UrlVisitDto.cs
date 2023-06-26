using System.Net;
using TracklyApi.Entities;

namespace TracklyApi.Dtos.Url;
public class UrlVisitDto
{
    public DateTime VisitTimestamp { get; set; }
    public IPAddress IpAddress { get; set; } = null!;
    public string? CountryCode { get; set; }
    public string? BrowserFingerprint { get; set; }
    public RedirectResults RedirectResult { get; set; }
}
