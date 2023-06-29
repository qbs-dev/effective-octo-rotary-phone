namespace TracklyApi.Dtos.Url;
public class UrlVisitDto
{
    public DateTime VisitTimestamp { get; set; }
    public string IpAddress { get; set; } = null!;
    public string CountryCode { get; set; } = null!;
}
