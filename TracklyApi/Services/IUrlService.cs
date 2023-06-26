using Ardalis.Result;
using Sieve.Models;
using TracklyApi.Dtos;
using TracklyApi.Dtos.Url;

namespace TracklyApi.Services;
public interface IUrlService
{
    public Task<Result<MessageResponseDto>> CreateUrlAsync(int userId, UrlDto url);
    public Task<Result<RedirectResultDto>> CompleteRedirectAsync(RedirectRequestDto redirectRequest);
    public Task<Result<PageDto<UrlShortDto>>> GetUrlsAsync(int userId, SieveModel sieveModel);
    public Task<Result<UrlDto>> GetUrlDetailsAsync(int userId, long urlId);
    public Task<Result<PageDto<UrlVisitDto>>> GetUrlVisitsAsync(int userId, long urlId, SieveModel sieveModel);
    public Task<Result<UrlDto>> EditUrlDetailsAsync(int userId, UrlDto url);
    public Task<Result<MessageResponseDto>> DeleteUrlAsync(int userId, long urlId);
}
