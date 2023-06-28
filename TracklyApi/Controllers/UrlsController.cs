using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;
using TracklyApi.Dtos;
using TracklyApi.Dtos.Url;
using TracklyApi.Services;

namespace TracklyApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UrlsController : ControllerBase
{
    private readonly IUrlService _urlService;
    public UrlsController(IUrlService urlService)
    {
        _urlService = urlService;
    }

    [Authorize(Policy = "CheckUserId")]
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<PageDto<UrlShortDto>>> GetUrls(int userId, string? filters, string? sorts, int page = 1, int pageSize = 10)
    {
        var sieveModel = new SieveModel
        {
            Filters = filters ?? "",
            Sorts = sorts ?? "",
            Page = page,
            PageSize = pageSize
        };
        return this.ToActionResult(await _urlService.GetUrlsAsync(userId, sieveModel));
    }

    [Authorize(Policy = "CheckUserId")]
    [HttpGet("{urlId}/visits")]
    public async Task<ActionResult<PageDto<UrlVisitDto>>> GetUrlVisits(int userId, int urlId,
        string? filters, string? sorts, int page = 1, int pageSize = 10)
    {
        var sieveModel = new SieveModel
        {
            Filters = filters ?? "",
            Sorts = sorts ?? "",
            Page = page,
            PageSize = pageSize
        };
        return this.ToActionResult(await _urlService.GetUrlVisitsAsync(userId, urlId, sieveModel));
    }


    [HttpGet("{urlId}")]
    public async Task<ActionResult<UrlDto>> GetUrlDetails(int userId, long urlId)
    {
        return this.ToActionResult(await _urlService.GetUrlDetailsAsync(userId, urlId));
    }

    [HttpPost("create")]
    public async Task<ActionResult<MessageResponseDto>> CreateUrl(int userId, UrlDto url)
    {
        return this.ToActionResult(await _urlService.CreateUrlAsync(userId, url));
    }

    [HttpPost("edit")]
    public async Task<ActionResult<UrlDto>> EditUrlDetails(int userId, UrlDto url)
    {
        return this.ToActionResult(await _urlService.EditUrlDetailsAsync(userId, url));
    }

    [HttpDelete("{urlId}")]
    public async Task<ActionResult<MessageResponseDto>> DeleteUrl(int userId, long urlId)
    {
        return this.ToActionResult(await _urlService.DeleteUrlAsync(userId, urlId));
    }
}
