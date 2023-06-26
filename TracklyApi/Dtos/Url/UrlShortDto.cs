namespace TracklyApi.Dtos.Url;
public class UrlShortDto
{
    public long Id { get; set; }
    public bool IsActive { get; set; }
    public string NewPath { get; set; } = null!;
    public string TargetUrl { get; set; } = null!;
    public long TotalClicks { get; set; }
}
