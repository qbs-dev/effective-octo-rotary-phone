namespace TracklyApi.Dtos.Url;
public class UrlEditRequestDto : UrlBaseDto
{
    public long Id { get; set; }
    public string Description { get; set; } = null!;
}
