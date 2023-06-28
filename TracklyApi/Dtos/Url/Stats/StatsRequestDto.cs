namespace TracklyApi.Dtos.Url.Stats;
public class StatsRequestDto
{
    public int UserId { get; set; }
    public long UrlId { get; set; }
    public int Limit { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
