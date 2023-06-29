namespace TracklyApi.Dtos.Url;
public class UrlBaseDto
{
    public bool IsActive { get; set; }
    public string NewPath { get; set; } = null!;
    public string TargetUrl { get; set; } = null!;
}
