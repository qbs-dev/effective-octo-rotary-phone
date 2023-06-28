using System.Net;
using Ardalis.Result;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using TracklyApi.Dtos;
using TracklyApi.Dtos.Url;
using TracklyApi.Entities;

namespace TracklyApi.Services.Implementations;
public class UrlService : IUrlService
{
    private readonly TracklyDbContext _context;
    private readonly IMapper _mapper;
    private readonly ISieveProcessor _sieve;
    private readonly IValidator<UrlDto> _urlValidator;
    private readonly IValidator<RedirectRequestDto> _redirectRequestValidator;
    private readonly ILogger<UrlService> _logger;

    public UrlService(TracklyDbContext context, IMapper mapper,
        ISieveProcessor sieve, IValidator<UrlDto> urlValidator,
        IValidator<RedirectRequestDto> redirectRequestValidator,
        ILogger<UrlService> logger)
    {
        _context = context;
        _mapper = mapper;
        _sieve = sieve;
        _urlValidator = urlValidator;
        _redirectRequestValidator = redirectRequestValidator;
        _logger = logger;
    }

    public async Task<Result<MessageResponseDto>> CreateUrlAsync(int userId, UrlDto url)
    {
        var validationResult = _urlValidator.Validate(url);
        if (!validationResult.IsValid)
            return Result.Error(validationResult.ToString(", "));
        var newUrl = _mapper.Map<UrlDto, ManagedUrl>(url);
        newUrl.User = userId;

        _context.ManagedUrls.Add(newUrl);
        var changedRows = await _context.SaveChangesAsync();
        if (changedRows >= 1)
        {
            return Result.Success(new MessageResponseDto("successfully added managed url"));
        }
        else
        {
            return Result.Error("internal server error");
        }
    }

    public async Task<Result<RedirectResultDto>> CompleteRedirectAsync(RedirectRequestDto redirectRequest)
    {
        DateTime visitDate = DateTime.UtcNow;

        var validationResult = _redirectRequestValidator.Validate(redirectRequest);
        if (!validationResult.IsValid)
            return Result.Error(validationResult.ToString(", "));

        IPAddress ipAddress = IPAddress.Parse(redirectRequest.IpAddressString);
        var managedUrl = await _context.ManagedUrls
            .Where(x => x.IsActive == true && x.NewPath.Equals(redirectRequest.Path)).FirstOrDefaultAsync();

        if (managedUrl == null)
            return Result.NotFound();

        UrlVisit newVisit = new UrlVisit
        {
            Url = managedUrl.Id,
            VisitTimestamp = visitDate,
            IpAddress = ipAddress
        };

        _context.UrlVisits.Add(newVisit);

        var changedRows = await _context.SaveChangesAsync();
        if (changedRows >= 1)
        {
            _logger.LogInformation($"{redirectRequest.IpAddressString} visited {redirectRequest.Path} at {visitDate}");
        }
        else
        {
            _logger.LogError($"Can't add visit record: {newVisit}");
        }

        return Result.Success(
                new RedirectResultDto
                {
                    TargetUrl = managedUrl.TargetUrl
                }
            );
    }

    public async Task<Result<MessageResponseDto>> DeleteUrlAsync(int userId, long urlId)
    {
        var url = await _context.ManagedUrls.FindAsync(urlId);
        if (url == null || url.User != userId)
            return Result.Error("url doesn't exist");

        _context.ManagedUrls.Remove(url);
        var result = await _context.SaveChangesAsync();

        return (result > 0) ?
            Result.Success(new MessageResponseDto("url successfully deleted")) :
            Result.Error("unable to delete url");
    }

    public async Task<Result<UrlDto>> EditUrlDetailsAsync(int userId, UrlDto url)
    {
        var validationResult = _urlValidator.Validate(url);
        if (!validationResult.IsValid)
        {
            return Result.Error(validationResult.ToString(", "));
        }

        var urlEntity = await _context.ManagedUrls
            .Where(x => x.Id == url.Id && x.User == userId).FirstOrDefaultAsync();
        if (urlEntity == null)
            return Result.Error("url doesn't exist");

        _mapper.Map<UrlDto, ManagedUrl>(url, urlEntity);

        _context.ManagedUrls.Update(urlEntity);

        var result = await _context.SaveChangesAsync();
        return result > 0 ? Result.Success(_mapper.Map<UrlDto>(urlEntity))
            : Result.Error("url details weren't modified");
    }

    public async Task<Result<UrlDto>> GetUrlDetailsAsync(int userId, long urlId)
    {
        var url = await _context.ManagedUrls.AsNoTracking()
            .Where(x => x.Id == urlId && x.User == userId)
            .FirstOrDefaultAsync();
        return url == null ?
            Result.Error("url doesn't exist") :
            Result.Success(_mapper.Map<UrlDto>(url));
    }

    public async Task<Result<PageDto<UrlShortDto>>> GetUrlsAsync(int userId, SieveModel sieveModel)
    {
        var urlsQueryRO = _context.ManagedUrls
                .Where(x => x.User == userId)
                .OrderByDescending(x => x.CreatedAt)
                .AsNoTracking();

        urlsQueryRO = _sieve.Apply(sieveModel, urlsQueryRO, applyPagination: false);

        var urlsCount = await urlsQueryRO.CountAsync();

        urlsQueryRO = _sieve.Apply(sieveModel, urlsQueryRO,
            applyFiltering: false, applySorting: false);

        var urls = await urlsQueryRO.ToArrayAsync();

        return Result.Success(new PageDto<UrlShortDto>
        {
            PageItems = _mapper.Map<UrlShortDto[]>(urls),
            TotalCount = urlsCount
        });
    }

    public async Task<Result<PageDto<UrlVisitDto>>> GetUrlVisitsAsync(int userId, long urlId, SieveModel sieveModel)
    {
        var urlVisitsQueryRO = _context.UrlVisits
                .Where(x => x.Url == urlId)
                .OrderByDescending(x => x.VisitTimestamp)
                .AsNoTracking();

        urlVisitsQueryRO = _sieve.Apply(sieveModel, urlVisitsQueryRO, applyPagination: false);

        var visitsCount = await urlVisitsQueryRO.CountAsync();

        urlVisitsQueryRO = _sieve.Apply(sieveModel, urlVisitsQueryRO,
            applyFiltering: false, applySorting: false);

        var visits = await urlVisitsQueryRO.ToArrayAsync();

        return Result.Success(new PageDto<UrlVisitDto>
        {
            PageItems = _mapper.Map<UrlVisitDto[]>(visits),
            TotalCount = visitsCount
        });
    }
}
