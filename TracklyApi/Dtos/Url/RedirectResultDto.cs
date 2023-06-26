using TracklyApi.Entities;

namespace TracklyApi.Dtos.Url;
public class RedirectResultDto
{
    public int MinTimeout { get; set; }
    public RedirectResults Result { get; set; }
    public string? TargetUrl { get; set; }
}
