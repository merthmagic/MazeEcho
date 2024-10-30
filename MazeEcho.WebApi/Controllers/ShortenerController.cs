using MazeEcho.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MazeEcho.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShortenerController(IGrainFactory grains) : ControllerBase
    {
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<string>> Shorten([BindRequired] string url)
        {
            var request = Request;
            var host = $"{request.Scheme}://{request.Host.Value}";

            if (string.IsNullOrWhiteSpace(url) ||
                Uri.IsWellFormedUriString(url, UriKind.Absolute) is false)
            {
                return BadRequest($"""
                                   The URL query string is required and needs to be well formed.
                                   Consider, ${host}/shorten?url=https://www.microsoft.com.
                                   """);
            }

            var shortenedRouteSegment = Guid.NewGuid().GetHashCode().ToString("X");
            var shortenerGrain =
                grains.GetGrain<IUrlShortenerGrain>(shortenedRouteSegment);
            await shortenerGrain.SetUrl(url);
            var resultBuilder = new UriBuilder(host)
            {
                Path = $"/go/{shortenedRouteSegment}"
            };
            return Ok(resultBuilder.Uri);
        }
    }
}