namespace TracklyApi.Dtos.Profile;
public class ProfileDto : ProfileBaseDto
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public bool? IsEmailVerified { get; set; }
    public DateTime RegistrationDate { get; set; }
    public DateTime LastAccessDate { get; set; }
}
