namespace TracklyApi.Dtos.Url.Stats;
public class VisitsByCountryDto
{
    public string CountryCode { get; set; } = null!;
    public int VisitsCount { get; set; }
}
