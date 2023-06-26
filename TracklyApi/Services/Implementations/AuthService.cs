using Ardalis.Result;
using AutoMapper;
using FluentValidation;
using TracklyApi.Dtos;
using TracklyApi.Dtos.Profile;
using TracklyApi.Dtos.Auth;
using TracklyApi.Entities;

namespace TracklyApi.Services.Implementations;
public class AuthService : IAuthService
{
    private readonly TracklyDbContext _context;
    private readonly IJWTService _jwtService;
    private readonly IValidator<AuthRequestDto> _authRequestValidator;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthService> _logger;

    public AuthService(TracklyDbContext context, IJWTService jwtService,
        IValidator<AuthRequestDto> authRequestValidator,
        IMapper mapper, ILogger<AuthService> logger)
    {
        _jwtService = jwtService;
        _context = context;
        _authRequestValidator = authRequestValidator;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(AuthRequestDto authRequest)
    {
        _logger.LogInformation($"Logging in to {authRequest.Email}");
        _logger.LogDebug($"Validating login request: {authRequest}");

        var validationResult = _authRequestValidator.Validate(authRequest);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation($"Invalid login request to {authRequest.Email}");
            _logger.LogDebug($"Validation errors: {validationResult},");
            return Result.Error(validationResult.ToString(", "));
        }

        return await _jwtService.GetAccessAsync(authRequest);
    }

    public async Task<Result<MessageResponseDto>> LogoutAsync(Guid refresh)
    {
        _logger.LogInformation($"Logging out from session [{refresh}]");
        _logger.LogDebug($"Finding session {refresh}");

        var sessionToDelete = await _context.RefreshSessions.FindAsync(refresh);
        if (sessionToDelete == null)
        {
            _logger.LogInformation($"Logout result [{refresh}]: session doesn't exist");
            return Result.Success(new MessageResponseDto("session doesn't exist"));
        }

        _logger.LogDebug($"Removing session {refresh}");
        _context.RefreshSessions.Remove(sessionToDelete);
        var changedRows = await _context.SaveChangesAsync();

        if (changedRows < 1)
        {
            _logger.LogError($"Logout result [{refresh}]: unable to remove session");
            return Result.Error("unable to remove session");
        }
        else
        {
            _logger.LogInformation($"Logout result [{refresh}]: success");
            return Result.Success(new MessageResponseDto("logged out"));
        }
    }


    public async Task<Result<AuthResponseDto>> RefreshExistingSessionAsync(string fingerprint, Guid refresh)
    {
        _logger.LogInformation($"Refreshing session [{refresh}]");
        _logger.LogDebug($"Finding session {refresh}");
        var refreshSession = await _context.RefreshSessions.FindAsync(refresh);

        if (refreshSession == null || !refreshSession.Fingerprint.Equals(fingerprint))
        {
            _logger.LogInformation($"Refresh result [{refresh}]: session doesn't exist");
            return Result.Error("session doesn't exist");
        }

        _logger.LogDebug($"Checking if session {refresh} expired");
        if (refreshSession.ExpiresIn < DateTime.UtcNow)
        {
            _logger.LogDebug($"Removing session {refresh} as it expired");
            _context.RefreshSessions.Remove(refreshSession);
            var changedRows = await _context.SaveChangesAsync();
            if (changedRows < 1)
            {
                _logger.LogError($"Unable to remove expired session {refresh}");
            }
            else
            {
                _logger.LogDebug($"Session {refresh} was removed");
            }
            _logger.LogInformation($"Refresh result [{refresh}]: session expired");
            return Result.Error("session expired");
        }
        AuthResponseDto refreshResponse = await _jwtService.GetRefreshAsync(
            refreshSession.User, refresh, fingerprint);

        return refreshResponse;
    }
}
