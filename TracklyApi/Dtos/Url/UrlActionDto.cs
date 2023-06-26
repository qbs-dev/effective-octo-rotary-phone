using TracklyApi.Entities;

namespace TracklyApi.Dtos.Url;
public class UrlActionDto
{
    public long Id { get; set; }
    public ActionTypes ActionType { get; set; }
    public object Value { get; set; } = null!;
}

