namespace TracklyApi.Dtos.Url.Stats;
public class StatsResponseDto<T>
{
    public UrlShortDto Url { get; set; } = null!;
    public int TotalVisitsCount { get; set; }
    public T[] Stats { get; set; } = null!;
}
