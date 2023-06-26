using TracklyApi.Services;
using TracklyApi.Dtos.Auth;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using TracklyApi.Dtos;

namespace TracklyApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService, IProfileService profileService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(AuthRequestDto signinData)
    {
        var authResponse = await _authService.LoginAsync(signinData);
        if (authResponse.IsSuccess)
        {
            Response.Cookies.Append("refresh", authResponse.Value.RefreshToken!,
            new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(30),
                Path = "/api/auth"
            });
            authResponse.Value.RefreshToken = "cookie";
        }

        return this.ToActionResult(authResponse);
    }

    [HttpPost("logout")]
    public async Task<ActionResult<MessageResponseDto>> Logout()
    {
        Guid refreshSession;
        bool parseRes = Guid.TryParse(Request.Cookies
            .SingleOrDefault(c => c.Key == "refresh").Value,
            out refreshSession);

        if (!parseRes)
            return this.ToActionResult(Result.Error("incorrect refresh token"));

        var signoutResult = await _authService.LogoutAsync(refreshSession);
        Response.Cookies.Delete("refresh");
        return this.ToActionResult(signoutResult);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponseDto>> RefreshExistingSession(string fingerprint)
    {
        Guid refreshToken;
        bool parseRes = Guid.TryParse(Request.Cookies
            .SingleOrDefault(c => c.Key == "refresh").Value,
            out refreshToken);

        if (!parseRes)
            return this.ToActionResult(Result.Error("incorrect refresh token"));

        var refreshResult = await _authService.RefreshExistingSessionAsync(fingerprint, refreshToken);

        if (refreshResult.IsSuccess)
        {
            Response.Cookies.Append("refresh", refreshResult.Value.RefreshToken!,
            new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(30),
                Path = "/api/auth"
            });
            refreshResult.Value.RefreshToken = "cookie";
        }
        else
        {
            Response.Cookies.Delete("refresh");
        }
        return this.ToActionResult(refreshResult);
    }
}
