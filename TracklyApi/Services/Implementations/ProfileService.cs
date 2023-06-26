using Ardalis.Result;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TracklyApi.Dtos;
using TracklyApi.Dtos.Profile;
using TracklyApi.Entities;

namespace TracklyApi.Services.Implementations;
public class ProfileService : IProfileService
{
    private readonly TracklyDbContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<RegisterDto> _registerValidator;
    private readonly ILogger<ProfileService> _logger;

    public ProfileService(TracklyDbContext context, IMapper mapper,
        IValidator<RegisterDto> registerValidator, ILogger<ProfileService> logger)
    {
        _context = context;
        _mapper = mapper;
        _registerValidator = registerValidator;
        _logger = logger;
    }

    public async Task<Result<MessageResponseDto>> RegisterAsync(RegisterDto regData)
    {
        _logger.LogInformation($"Registering new profile: {regData.Email}");
        _logger.LogDebug($"Validating register request: {regData}");
        var validationResult = _registerValidator.Validate(regData);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation($"Invalid register request {regData.Email}");
            _logger.LogDebug($"Validation errors: {validationResult},");
            return Result.Error(validationResult.ToString(", "));
        }

        var newUser = _mapper.Map<RegisterDto, User>(regData);


        _logger.LogDebug($"Searching for profile with email {regData.Email}");
        //reject if account with this email exists
        var userExists = await _context.Users.AsNoTracking().Where(
            u => u.Email == regData.Email).FirstOrDefaultAsync() != null;
        if (userExists)
        {
            _logger.LogInformation($"Register result: email {regData.Email} already in use");
            return Result.Error("email already in use");
        }

        _logger.LogDebug($"Adding new profile: {newUser}");
        _context.Users.Add(newUser);
        var changedRows = await _context.SaveChangesAsync();

        if (changedRows == 1)
        {
            _logger.LogInformation($"Register result for {regData.Email}: success");
            return Result.Success(new MessageResponseDto("successfully regstered"));
        }
        else
        {
            _logger.LogError($"Can't add new profile");
            return Result.Error("internal service error");
        }
    }

    public async Task<Result<UserDto>> GetProfileDetailsAsync(int id)
    {
        _logger.LogInformation($"Getting profile details: {id}");
        var user = await _context.Users.AsNoTracking()
            .Where(x => x.Id == id).FirstOrDefaultAsync();
        return user == null ?
            Result.Error("profile doesn't exist") :
            Result.Success(_mapper.Map<UserDto>(user));
    }

    public async Task<Result<UserDto>> EditProfileDetailsAsync(int userId, ProfileDetailsDto profileDetails)
    {
        var userEntity = await _context.Users.FindAsync(userId);
        if (userEntity == null)
            return Result.Error("profile doesn't exist");

        _mapper.Map<ProfileDetailsDto, User>(profileDetails, userEntity);

        _context.Users.Update(userEntity);

        var result = await _context.SaveChangesAsync();
        return result > 0 ? Result.Success(_mapper.Map<UserDto>(userEntity))
            : Result.Error("error while changing profile details");
    }
    public async Task<Result<MessageResponseDto>> DeleteProfileAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return Result.Error("profile doesn't exist");

        _context.Users.Remove(user);
        var result = await _context.SaveChangesAsync();

        return (result > 0) ?
            Result.Success(new MessageResponseDto("profile successfully deleted")) :
            Result.Error("unable to delete profile");
    }
}
