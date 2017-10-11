using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UrlCompressorService.Services;

namespace UrlCompressorService.Controllers
{
    [Route("rt")]
    public class RouteController : Controller
    {
        private readonly UrlEntriesProvider _urlEntriesProvider;

        public RouteController(UrlEntriesProvider urlEntriesProvider)
        {
            _urlEntriesProvider = urlEntriesProvider;
        }

        [HttpGet("{url}")]
        public async Task<IActionResult> MyRedirect(string url)
        {
            var urlEntry = _urlEntriesProvider.GetUrlEntryByShortUrl(url);
            if (urlEntry == null)
                return NotFound();

            urlEntry.RedirectCount++;
            await _urlEntriesProvider.UpdateUrlEntry(urlEntry);
            
            return base.Redirect(urlEntry.LongUrl);
        }
    }
}