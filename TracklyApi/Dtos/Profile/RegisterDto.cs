namespace TracklyApi.Dtos.Profile;
public class RegisterDto : ProfileDetailsDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
