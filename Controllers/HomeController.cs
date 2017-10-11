using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UrlCompressorService.Models;
using UrlCompressorService.Services;

namespace UrlCompressorService.Controllers
{
    public class HomeController : Controller
    {
        private const string UserKey = "user_id";
        private UrlEntriesProvider _urlEntriesProvider;

        public HomeController(UrlEntriesProvider urlEntriesProvider)
        {
            _urlEntriesProvider = urlEntriesProvider;
        }

        public async Task<IActionResult> Index()
        {
            var guid = HttpContext.Request.Cookies[UserKey];
            return View(await _urlEntriesProvider.GetUrlEntries(guid));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LongUrl")] UrlEntry urlEntry)
        {
            if (ModelState.IsValid)
            {
                var oldUrEntry = _urlEntriesProvider.GetUrlEntry(urlEntry.LongUrl);
                
                if (oldUrEntry != null)
                {
                    return View(oldUrEntry);
                }

                urlEntry.Guid = HttpContext.Request.Cookies[UserKey];
                if (urlEntry.Guid == null)
                {
                    urlEntry.Guid = Guid.NewGuid().ToString();
                    HttpContext.Response.Cookies.Append(UserKey, urlEntry.Guid, new Microsoft.AspNetCore.Http.CookieOptions()
                    {
                        Path = "/",
                        Expires = DateTime.Now.AddDays(7)
                    });
                }

                await _urlEntriesProvider.AddAndInitUrlEntry(urlEntry);

                return View(urlEntry);

            }

            return View(urlEntry);
        }
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var urlEntry = await _urlEntriesProvider.GetUrlEntry(id.Value);
            if (urlEntry == null)
            {
                return NotFound();
            }

            return View(urlEntry);
        }
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _urlEntriesProvider.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
