using Ardalis.Result;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TracklyApi.Entities;
using TracklyApi.Dtos.Auth;

namespace TracklyApi.Services.Implementations;
public class JWTService : IJWTService
{
    private readonly TracklyDbContext _context;
    private readonly IConfiguration _config;
    private readonly ILogger<JWTService> _logger;

    public JWTService(TracklyDbContext context,
        IConfiguration config, ILogger<JWTService> logger)
    {
        _context = context;
        _config = config;
        _logger = logger;
    }

    public async Task<Result<AuthResponseDto>> GetAccessAsync(AuthRequestDto authRequest)
    {
        _logger.LogInformation($"Authenticating {authRequest.Email}");
        _logger.LogDebug($"Searching for profile {authRequest.Email}:password_hash");

        var user = _context.Users.FirstOrDefault(
            u => u.Email.Equals(authRequest.Email)
            && u.Password.Equals(authRequest.Password));

        if (user is null)
        {
            _logger.LogInformation($"Login result: incorrect email or password");
            return Result.Error("incorrect email or password");
        }

        _logger.LogInformation($"Creating session for {authRequest.Email}");
        _logger.LogDebug($"Generating access token for {authRequest.Email}");
        var newAccessToken = GenerateToken(user.Id, authRequest);
        return await SaveTokenDetails(user, authRequest.Fingerprint,
            newAccessToken);
    }

    public async Task<Result<AuthResponseDto>> GetRefreshAsync(
        int userId, Guid oldRefresh, string fingerprint)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user is null)
            return Result.Error("requested profile doesn't exist");

        var accessToken = GenerateToken(userId,
            new AuthRequestDto { Email = user.Email, Fingerprint = fingerprint });
        return await SaveTokenDetails(user, fingerprint,
            accessToken);
    }

    public async Task<bool> ValidateAccessAsync(JwtSecurityToken token)
    {
        var userIdString = token.Claims.FirstOrDefault(x => x.Type == "userId");
        var fingerprint = token.Claims.FirstOrDefault(x => x.Type == "fingerprint");

        if (userIdString == null || fingerprint == null)
            return false;

        int userId;
        if (!int.TryParse(userIdString.Value, out userId))
            return false;

        var refreshSession = await _context.RefreshSessions.Where(
            x => x.User == userId && x.Fingerprint.Equals(fingerprint)
        ).FirstOrDefaultAsync();

        return refreshSession != null;
    }

    private string GenerateToken(int userId, AuthRequestDto authRequest)
    {
        var jwtKeyBytes = Encoding.ASCII.GetBytes(_config["JWT:Secret"]!);
        var tokenHandler = new JwtSecurityTokenHandler();
        var descriptor = new SecurityTokenDescriptor()
        {
            Audience = _config["JWT:ValidAudience"],
            Issuer = _config["JWT:ValidIssuers:0"],
            Subject = new ClaimsIdentity(new Claim[]
            {
                    new Claim("userId", userId.ToString()),
                    new Claim("fingerprint", authRequest.Fingerprint),

            }),
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(5),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(jwtKeyBytes),
                SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(descriptor);
        _logger.LogDebug($"Generated token: {token}");
        var tokenString = tokenHandler.WriteToken(token);
        return tokenString;
    }

    private async Task<Result<AuthResponseDto>> SaveTokenDetails(
        User user, string fingerprint, string accessToken)
    {
        Guid refreshToken;
        //Check if refresh session with given user id and fingerprint exists

        _logger.LogDebug($"Getting list of user sessions");
        var activeSessions = await _context.RefreshSessions
            .Where(rs => rs.User == user.Id).ToListAsync();

        _logger.LogDebug($"Checking if session for given client device already exists");
        var existingSession = activeSessions
            .Where(rs => rs.Fingerprint == fingerprint).FirstOrDefault();

        //Session already exists, update expiry time
        if (existingSession != null)
        {
            _logger.LogDebug("Refreshing existing session");
            refreshToken = existingSession.RefreshToken;
            existingSession.ExpiresIn = DateTime.UtcNow.AddDays(30);
        }
        else
        {
            refreshToken = Guid.NewGuid();
            //check if user already has 5 sessions
            if (activeSessions.Count >= 5)
            {
                _logger.LogDebug("User has 5 or more active sessions. Removing all sessions");
                _context.RemoveRange(activeSessions);
            }

            var newRefreshSession = new RefreshSession
            {
                UserNavigation = user,
                Fingerprint = fingerprint,
                RefreshToken = refreshToken,
                CreatedAt = DateTime.UtcNow,
                ExpiresIn = DateTime.UtcNow.AddDays(30),
            };

            _logger.LogDebug($"Created new session: {newRefreshSession}");
            _context.Add(newRefreshSession);
        }
        user.LastAccessDate = DateTime.UtcNow;

        _logger.LogDebug($"Saving changes");
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Login to {user.Email} result: success");
        return Result.Success(new AuthResponseDto
        {
            Message = $"welcome, {user.FirstName}",
            AccessToken = accessToken,
            RefreshToken = refreshToken.ToString()
        });
    }
}
