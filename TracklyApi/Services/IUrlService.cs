using Ardalis.Result;
using Sieve.Models;
using TracklyApi.Dtos;
using TracklyApi.Dtos.Url;
using TracklyApi.Dtos.Url.Stats;

namespace TracklyApi.Services;
public interface IUrlService
{
    public Task<Result<MessageResponseDto>> CreateUrlAsync(int userId, UrlEditRequestDto newUrlRequest);
    public Task<Result<PageDto<UrlShortDto>>> GetUrlsAsync(int userId, SieveModel sieveModel);
    public Task<Result<UrlDto>> GetUrlDetailsAsync(int userId, long urlId);
    public Task<Result<PageDto<UrlVisitDto>>> GetUrlVisitsAsync(int userId, long urlId, SieveModel sieveModel);
    public Task<Result<StatsResponseDto<VisitsByCountryDto>>> GetUrlVisitsByCountryAsync(StatsRequestDto statsRequest);
    public Task<Result<StatsResponseDto<VisitsByIpAddressDto>>> GetUrlVisitsByIpAddressAsync(StatsRequestDto statsRequest);
    public Task<Result<UrlDto>> EditUrlDetailsAsync(int userId, UrlEditRequestDto editUrlRequest);
    public Task<Result<MessageResponseDto>> DeleteUrlAsync(int userId, long urlId);
    public Task<string> GetCountryByIpAddressAsync(string ipAddress);
    public Task<Result<RedirectResultDto>> PerformRedirectAsync(RedirectRequestDto redirectRequest);
}
