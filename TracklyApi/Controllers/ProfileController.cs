using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using TracklyApi.Dtos;
using TracklyApi.Services;
using TracklyApi.Dtos.Profile;
using Microsoft.AspNetCore.Authorization;

namespace ZappAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;
    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<MessageResponseDto>> Register(RegisterDto regData)
    {
        return this.ToActionResult(await _profileService.RegisterAsync(regData));
    }

    [Authorize(Policy = "CheckUserId")]
    [HttpGet("{userId}")]
    public async Task<ActionResult<ProfileDto>> GetProfileDetails(int userId)
    {
        return this.ToActionResult(await _profileService.GetProfileDetailsAsync(userId));
    }

    [Authorize(Policy = "CheckUserId")]
    [HttpPost("{userId}/edit")]
    public async Task<ActionResult<ProfileDto>> EditProfileDetails(int userId, ProfileBaseDto profileDetails)
    {
        return this.ToActionResult(await _profileService.EditProfileDetailsAsync(userId, profileDetails));
    }

    [Authorize(Policy = "CheckUserId")]
    [HttpDelete("{userId}")]
    public async Task<ActionResult<MessageResponseDto>> DeleteProfile(int userId)
    {
        return this.ToActionResult(await _profileService.DeleteProfileAsync(userId));
    }
}
