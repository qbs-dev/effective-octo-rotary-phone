using Ardalis.Result;
using TracklyApi.Dtos;
using TracklyApi.Dtos.Auth;

namespace TracklyApi.Services;
public interface IAuthService
{
    public Task<Result<AuthResponseDto>> LoginAsync(AuthRequestDto authRequest);
    public Task<Result<MessageResponseDto>> LogoutAsync(Guid refresh);
    public Task<Result<AuthResponseDto>> RefreshExistingSessionAsync(string fingerprint, Guid refresh);
}
