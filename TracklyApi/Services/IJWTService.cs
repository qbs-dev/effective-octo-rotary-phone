using System.IdentityModel.Tokens.Jwt;
using Ardalis.Result;
using TracklyApi.Dtos.Auth;

namespace TracklyApi.Services;
public interface IJWTService
{
    Task<Result<AuthResponseDto>> GetAccessAsync(AuthRequestDto authRequest);
    Task<Result<AuthResponseDto>> GetRefreshAsync(int userId, Guid oldRefresh, string fingerprint);
    Task<bool> ValidateAccessAsync(JwtSecurityToken token);
}
