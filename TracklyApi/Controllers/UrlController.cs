using System;
using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;
using TracklyApi.Dtos;
using TracklyApi.Dtos.Url;
using TracklyApi.Services;

namespace TracklyApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UrlController : ControllerBase
{
    private readonly IUrlService _urlService;
    public UrlController(IUrlService urlService)
    {
        _urlService = urlService;
    }

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

    [HttpGet("{**newPath}")]
    public async Task<ActionResult<RedirectResultDto>> PerformRedirectAction(string newPath, string? ipAddress, string? fingerprint)
    {
        var result = await _urlService.CompleteRedirectAsync(new RedirectRequestDto
        {
            Path = newPath,
            IpAddressString = ipAddress ?? Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "",
            Fingerprint = fingerprint ?? ""
        });

        if (result.IsSuccess)
        {
            return new RedirectResult(result.Value.TargetUrl!);
        }

        return this.ToActionResult(result);
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
