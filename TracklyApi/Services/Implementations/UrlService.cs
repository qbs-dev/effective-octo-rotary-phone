using System.Net;
using Ardalis.Result;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using TracklyApi.Dtos;
using TracklyApi.Dtos.Url;
using TracklyApi.Dtos.Url.Stats;
using TracklyApi.Entities;

namespace TracklyApi.Services.Implementations;
public class UrlService : IUrlService
{
    private readonly TracklyDbContext _context;
    private readonly IMapper _mapper;
    private readonly ISieveProcessor _sieve;
    private readonly IValidator<UrlEditRequestDto> _urlEditRequestValidator;
    private readonly IValidator<RedirectRequestDto> _redirectRequestValidator;
    private readonly ILogger<UrlService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public UrlService(TracklyDbContext context, IMapper mapper,
        ISieveProcessor sieve, IValidator<UrlEditRequestDto> urlEditRequestValidator,
        IValidator<RedirectRequestDto> redirectRequestValidator,
        ILogger<UrlService> logger, IHttpClientFactory httpClientFactory
        )
    {
        _context = context;
        _mapper = mapper;
        _sieve = sieve;
        _urlEditRequestValidator = urlEditRequestValidator;
        _redirectRequestValidator = redirectRequestValidator;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<Result<UrlDto>> CreateUrlAsync(int userId, UrlEditRequestDto newUrlRequest)
    {
        var validationResult = _urlEditRequestValidator.Validate(newUrlRequest);
        if (!validationResult.IsValid)
            return Result.Error(validationResult.ToString(", "));

        //check if url with give path already exists
        var managedUrl = await this.GetUrlByPathAsync(newUrlRequest.NewPath);
        if (managedUrl != null)
        {
            return Result.Error("Url with given path already exists");
        }

        var newUrl = _mapper.Map<UrlEditRequestDto, ManagedUrl>(newUrlRequest);
        newUrl.User = userId;

        _context.ManagedUrls.Add(newUrl);
        var changedRows = await _context.SaveChangesAsync();
        if (changedRows > 0)
        {
            return Result.Success(_mapper.Map<UrlDto>(newUrl));
        }
        else
        {
            return Result.Error("internal server error");
        }
    }

    public async Task<Result<MessageResponseDto>> DeleteUrlAsync(int userId, long urlId)
    {
        var url = await _context.ManagedUrls.FindAsync(urlId);
        if (url == null || url.User != userId)
            return Result.Error("url doesn't exist");

        _context.ManagedUrls.Remove(url);
        var changedRows = await _context.SaveChangesAsync();

        return (changedRows > 0) ?
            Result.Success(new MessageResponseDto("url successfully deleted")) :
            Result.Error("unable to delete url");
    }

    public async Task<Result<UrlDto>> EditUrlDetailsAsync(int userId, UrlEditRequestDto editUrlRequest)
    {
        var validationResult = _urlEditRequestValidator.Validate(editUrlRequest);
        if (!validationResult.IsValid)
        {
            return Result.Error(validationResult.ToString(", "));
        }

        var urlEntity = await _context.ManagedUrls
            .Where(x => x.Id == editUrlRequest.Id && x.User == userId).FirstOrDefaultAsync();
        if (urlEntity == null)
            return Result.Error("url doesn't exist");

        //check if url with give path already exists
        if (!editUrlRequest.NewPath.Equals(urlEntity.NewPath))
        {
            var managedUrl = await this.GetUrlByPathAsync(editUrlRequest.NewPath);
            if (managedUrl != null)
            {
                return Result.Error("Url with given path already exists");
            }
        }

        _mapper.Map<UrlEditRequestDto, ManagedUrl>(editUrlRequest, urlEntity);

        _context.ManagedUrls.Update(urlEntity);

        var changedRows = await _context.SaveChangesAsync();
        return changedRows > 0 ? Result.Success(_mapper.Map<UrlDto>(urlEntity))
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

    public async Task<string> GetCountryByIpAddressAsync(string ipAddress)
    {
        _logger.LogInformation($"Getting country code for {ipAddress}");

        try
        {
            var httpClient = _httpClientFactory.CreateClient("Ip2Geo");
            _logger.LogDebug($"Sending request to Ip2GeoApi endpoint");
            var httpResponse = await httpClient.GetAsync($"ip/{ipAddress}");

            _logger.LogDebug($"Got response from Ip2GeoApi: {httpResponse.IsSuccessStatusCode}");
            string countryCode = httpResponse.IsSuccessStatusCode ?
                (await httpResponse.Content.ReadFromJsonAsync<string>() ?? "ZZ") :
                "ZZ";

            _logger.LogInformation($"{ipAddress} - {countryCode}");
            return countryCode;
        }
        catch
        {
            _logger.LogError($"Cant get country code from Ip2GeoApi. Falling back to ${ipAddress} - ZZ");
            return "ZZ";
        }

    }

    public async Task<ManagedUrl?> GetUrlByPathAsync(string urlPath, bool isActiveOnly = false)
    {
        var managedUrlContext = isActiveOnly ?
        _context.ManagedUrls.Where(x => x.IsActive == true) :
        _context.ManagedUrls;

        var managedUrl = await managedUrlContext
            .Where(x => EF.Functions.ILike(x.NewPath, urlPath))
            .FirstOrDefaultAsync();
        return managedUrl;
    }


    public async Task<Result<UrlDto>> FindUrlByPathAsync(string urlPath)
    {
        var managedUrl = await this.GetUrlByPathAsync(urlPath);
        if (managedUrl == null)
            return Result.NotFound();
        return _mapper.Map<UrlDto>(managedUrl);
    }

    public async Task<Result<RedirectResultDto>> PerformRedirectAsync(RedirectRequestDto redirectRequest)
    {
        DateTime visitDate = DateTime.UtcNow;
        _logger.LogInformation($"Performing redirect on path:{redirectRequest.Path}, ip:{redirectRequest.IpAddressString}");

        var validationResult = _redirectRequestValidator.Validate(redirectRequest);
        if (!validationResult.IsValid)
            return Result.Error(validationResult.ToString(", "));

        IPAddress ipAddress = IPAddress.Parse(redirectRequest.IpAddressString);
        string countryCode = await this.GetCountryByIpAddressAsync(redirectRequest.IpAddressString);

        var managedUrl = await this.GetUrlByPathAsync(redirectRequest.Path, true);
        if (managedUrl == null)
        {
            return Result.NotFound();
        }


        managedUrl.TotalClicks++;
        UrlVisit newVisit = new UrlVisit
        {
            Url = managedUrl.Id,
            VisitTimestamp = visitDate,
            IpAddress = ipAddress,
            CountryCode = countryCode
        };

        _context.UrlVisits.Add(newVisit);
        _context.ManagedUrls.Update(managedUrl);

        var changedRows = await _context.SaveChangesAsync();
        if (changedRows > 0)
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

    public async Task<Result<StatsResponseDto<VisitsByCountryDto>>> GetUrlVisitsByCountryAsync(StatsRequestDto statsRequest)
    {
        var url = await _context.ManagedUrls.AsNoTracking()
            .Where(x => x.Id == statsRequest.UrlId && x.User == statsRequest.UserId)
            .FirstOrDefaultAsync();
        if (url == null)
            Result.Error("url doesn't exist");

        var urlShort = _mapper.Map<UrlShortDto>(url);

        var rankedVisits = await _context.UrlVisits.AsNoTracking()
            .Where(x => x.Url == statsRequest.UrlId)
            .Where(x => x.VisitTimestamp >= statsRequest.StartDate
                && x.VisitTimestamp <= statsRequest.EndDate)
            .GroupBy(x => x.CountryCode)
            .Select(
                g => new VisitsByCountryDto()
                {
                    CountryCode = g.Key,
                    VisitsCount = g.Count()
                }
            )
            .OrderByDescending(g => g.VisitsCount)
            .ToListAsync();

        if (rankedVisits == null)
            return Result.Error("can't get statistics");

        var totalVisitsCount = rankedVisits.Count();

        if (statsRequest.Limit > 0 && totalVisitsCount > statsRequest.Limit)
        {
            VisitsByCountryDto otherVisits = new()
            {
                CountryCode = "others",
                VisitsCount = rankedVisits
                    .Skip(5).Sum(x => x.VisitsCount)
            };

            rankedVisits = rankedVisits.Take(statsRequest.Limit).Append(otherVisits).ToList();
        }

        return Result.Success(new StatsResponseDto<VisitsByCountryDto>()
        {
            Url = urlShort,
            TotalVisitsCount = totalVisitsCount,
            Stats = rankedVisits.ToArray()
        });
    }

    public async Task<Result<StatsResponseDto<VisitsByIpAddressDto>>> GetUrlVisitsByIpAddressAsync(StatsRequestDto statsRequest)
    {
        var url = await _context.ManagedUrls.AsNoTracking()
            .Where(x => x.Id == statsRequest.UrlId && x.User == statsRequest.UserId)
            .FirstOrDefaultAsync();
        if (url == null)
            Result.Error("url doesn't exist");

        var urlShort = _mapper.Map<UrlShortDto>(url);

        var rankedVisits = await _context.UrlVisits.AsNoTracking()
            .Where(x => x.Url == statsRequest.UrlId)
            .Where(x => x.VisitTimestamp >= statsRequest.StartDate
                && x.VisitTimestamp <= statsRequest.EndDate)
            .GroupBy(x => x.IpAddress)
            .Select(
                g => new VisitsByIpAddressDto()
                {
                    IpAddress = g.Key.ToString(),
                    VisitsCount = g.Count()
                }
            )
            .OrderByDescending(g => g.VisitsCount)
            .ToListAsync();

        if (rankedVisits == null)
            return Result.Error("can't get statistics");

        var totalVisitsCount = rankedVisits.Count();

        if (statsRequest.Limit > 0 && totalVisitsCount > statsRequest.Limit)
        {
            VisitsByIpAddressDto otherVisits = new()
            {
                IpAddress = "others",
                VisitsCount = rankedVisits
                    .Skip(5).Sum(x => x.VisitsCount)
            };

            rankedVisits = rankedVisits.Take(statsRequest.Limit).Append(otherVisits).ToList();
        }

        return Result.Success(new StatsResponseDto<VisitsByIpAddressDto>()
        {
            Url = urlShort,
            TotalVisitsCount = totalVisitsCount,
            Stats = rankedVisits.ToArray()
        });
    }
}
