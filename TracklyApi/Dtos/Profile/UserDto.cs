namespace TracklyApi.Dtos.Profile;
public class UserDto : ProfileDetailsDto
{
    public int Id { get; set; }
    public bool? IsEmailVerified { get; set; }
    public DateTime RegistrationDate { get; set; }
    public DateTime LastAccessDate { get; set; }
}
