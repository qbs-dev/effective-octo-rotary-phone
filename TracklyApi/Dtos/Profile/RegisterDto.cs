namespace TracklyApi.Dtos.Profile;
public class RegisterDto : ProfileBaseDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
