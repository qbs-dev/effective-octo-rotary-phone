namespace TracklyApi.Dtos.Url.Stats;
public class VisitsByIpAddressDto
{
    public string IpAddress { get; set; } = null!;
    public int VisitsCount { get; set; }
}
