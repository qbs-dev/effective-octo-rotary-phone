namespace TracklyApi.Dtos.Auth;
public class AuthResponseDto
{
    public string Message { get; set; } = null!;
    public string AccessToken { get; set; } = null!;
    public string? RefreshToken { get; set; }
}
