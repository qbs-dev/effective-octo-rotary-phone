namespace TracklyApi.Dtos;

public class PageDto<T>
{
    public T[] PageItems { get; set; } = null!;
    public int TotalCount { get; set; }
}
