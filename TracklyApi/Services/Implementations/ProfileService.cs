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
    private readonly IValidator<ProfileBaseDto> _profileBaseValidator;
    private readonly ILogger<ProfileService> _logger;

    public ProfileService(TracklyDbContext context, IMapper mapper,
        IValidator<RegisterDto> registerValidator,
        IValidator<ProfileBaseDto> profileBaseValidator, ILogger<ProfileService> logger)
    {
        _context = context;
        _mapper = mapper;
        _registerValidator = registerValidator;
        _profileBaseValidator = profileBaseValidator;
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

        if (changedRows > 0)
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

    public async Task<Result<ProfileDto>> GetProfileDetailsAsync(int id)
    {
        _logger.LogInformation($"Getting profile details: {id}");
        var user = await _context.Users.AsNoTracking()
            .Where(x => x.Id == id).FirstOrDefaultAsync();

        if (user == null)
        {
            _logger.LogInformation($"Profile {id} doesn't exist");
            return Result.Error("profile doesn't exist");
        }
        else
        {
            _logger.LogInformation($"Returning mapped profile details");
            return Result.Success(_mapper.Map<ProfileDto>(user));
        }
    }

    public async Task<Result<ProfileDto>> EditProfileDetailsAsync(int userId, ProfileBaseDto profileDetails)
    {

        _logger.LogInformation($"Editing profile details for user: {userId}");
        _logger.LogDebug($"Validating edited details");
        var validationResult = _profileBaseValidator.Validate(profileDetails);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation($"Invalid profile details for user {userId}");
            _logger.LogDebug($"Validation errors: {validationResult},");
            return Result.Error(validationResult.ToString(", "));
        }

        _logger.LogDebug($"Searching for profile {userId}");
        var userEntity = await _context.Users.FindAsync(userId);
        if (userEntity == null)
        {
            _logger.LogInformation($"Profile {userId} doesn't exist");
            return Result.Error("profile doesn't exist");
        }

        _logger.LogDebug($"Mapping edited profile details to existing entity");
        _mapper.Map<ProfileBaseDto, User>(profileDetails, userEntity);

        _context.Users.Update(userEntity);

        _logger.LogDebug($"Saving changes to profile {userId}");
        var changedRows = await _context.SaveChangesAsync();

        if (changedRows > 0)
        {
            _logger.LogInformation($"Changed details for profile {userId}");
            return Result.Success(_mapper.Map<ProfileDto>(userEntity));
        }
        else
        {
            _logger.LogError($"Can't save changes for profile {userId}");
            return Result.Error("error while changing profile details");
        }
    }
    public async Task<Result<MessageResponseDto>> DeleteProfileAsync(int userId)
    {
        _logger.LogInformation($"Deleting profile: {userId}");
        _logger.LogDebug($"Searching for profile {userId}");
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            _logger.LogInformation($"Profile {userId} doesn't exist");
            return Result.Error("profile doesn't exist");
        }

        _logger.LogDebug($"Removing profile {userId} from database");
        _context.Users.Remove(user);
        var changedRows = await _context.SaveChangesAsync();

        if (changedRows > 0)
        {
            _logger.LogInformation($"Profile {userId} successfully deleted");
            return Result.Success(new MessageResponseDto("profile successfully deleted"));
        }
        else
        {
            _logger.LogError($"Can't remove profile {userId} from database");
            return Result.Error("unable to delete profile");
        }
    }
}
