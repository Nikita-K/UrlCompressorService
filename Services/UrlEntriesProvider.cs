using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System;
using System.Linq;
using System.Threading.Tasks;
using UrlCompressorService.Models;
using UrlCompressorService.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace UrlCompressorService.Services
{
    public class UrlEntriesProvider
    {
        private UrlEntryContext _urlEntriesContext;
        private readonly string _serviceUrl = "http://urlcm.azurewebsites.net/rt/";

        public UrlEntriesProvider(UrlEntryContext urlEntriesContext)
        {
            _urlEntriesContext = urlEntriesContext;
        }

        public UrlEntry GetUrlEntry(string url)
        {
            if (_urlEntriesContext.UrlEntries.Count() == 0)
                return null;

            return _urlEntriesContext
                 .UrlEntries
                 .FirstOrDefault(x => x.LongUrl == url);
        }

        public Task<List<UrlEntry>> GetUrlEntries(string guid)
        {
            return _urlEntriesContext.UrlEntries.Where(entry => entry.Guid == guid).ToListAsync();
        }

        public Task<UrlEntry> GetUrlEntry(int id)
        {
            return _urlEntriesContext.UrlEntries.SingleOrDefaultAsync(entry => entry.ID == id);
        }

        public UrlEntry GetUrlEntryByShortUrl(string url)
        {
            if (_urlEntriesContext.UrlEntries.Count() == 0)
                return null;

            var id = IdCryptographer.Decode(url);

            return _urlEntriesContext.UrlEntries.FirstOrDefault(x => x.ID == id);
        }

        public async Task AddAndInitUrlEntry(UrlEntry urlEntry)
        {
            _urlEntriesContext.Add(urlEntry);
            await _urlEntriesContext.SaveChangesAsync();

            InitUrlEntry(urlEntry);

            await UpdateUrlEntry(urlEntry);
        }

        public async Task UpdateUrlEntry(UrlEntry urlEntry)
        {
            _urlEntriesContext.Update(urlEntry);
            await _urlEntriesContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var urlEntry = await _urlEntriesContext.UrlEntries.SingleOrDefaultAsync(m => m.ID == id);
            _urlEntriesContext.UrlEntries.Remove(urlEntry);
            await _urlEntriesContext.SaveChangesAsync();
        }

        private void InitUrlEntry(UrlEntry urlEntry)
        {
            urlEntry.ShortUrl = _serviceUrl + IdCryptographer.Encode(urlEntry.ID);
            urlEntry.RedirectCount = 0;
            urlEntry.CreationDate = DateTime.Now;
        }

    }
}
