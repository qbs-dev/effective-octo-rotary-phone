namespace TracklyApi.Dtos.Url;
public class UrlDto : UrlShortDto
{
    public string Description { get; set; } = null!;
    public string CreatedAt { get; set; } = null!;
    public UrlActionDto[] Actions { get; set; } = null!;
}
