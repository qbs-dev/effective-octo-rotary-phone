using Ardalis.Result;
using TracklyApi.Dtos.Profile;
using TracklyApi.Dtos;

namespace TracklyApi.Services;
public interface IProfileService
{
    public Task<Result<MessageResponseDto>> RegisterAsync(RegisterDto regData);
    public Task<Result<UserDto>> GetProfileDetailsAsync(int id);
    public Task<Result<UserDto>> EditProfileDetailsAsync(int userId, ProfileDetailsDto profileDetails);
    public Task<Result<MessageResponseDto>> DeleteProfileAsync(int userId);

}
