using Ardalis.Result;
using TracklyApi.Dtos.Profile;
using TracklyApi.Dtos;

namespace TracklyApi.Services;
public interface IProfileService
{
    public Task<Result<MessageResponseDto>> RegisterAsync(RegisterDto regData);
    public Task<Result<ProfileDto>> GetProfileDetailsAsync(int id);
    public Task<Result<ProfileDto>> EditProfileDetailsAsync(int userId, ProfileBaseDto profileDetails);
    public Task<Result<MessageResponseDto>> DeleteProfileAsync(int userId);

}
