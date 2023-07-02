using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using TracklyApi.Dtos.Url;
using TracklyApi.Services;

namespace TracklyApi.Controllers;
[ApiController]
public class RedirectController : ControllerBase
{
    private readonly IUrlService _urlService;
    public RedirectController(IUrlService urlService)
    {
        _urlService = urlService;
    }

    [HttpGet("{**newPath:maxlength(64)}")]
    public async Task<ActionResult<RedirectResultDto>> PerformRedirectAction(string newPath)
    {

        var result = await _urlService.PerformRedirectAsync(new RedirectRequestDto
        {
            Path = newPath,
            IpAddressString = Request.HttpContext.Connection.RemoteIpAddress!.ToString()
        });

        if (result.IsSuccess)
        {
            return new RedirectResult(result.Value.TargetUrl!);
        }

        return this.ToActionResult(result);
    }
}
