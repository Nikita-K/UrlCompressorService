using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UrlCompressorService.Models.Interfaces;
using System.Threading.Tasks;

namespace UrlCompressorService.Models
{
    public class UrlEntry
    {
        [Key]
        public int ID { get; set; }
        
        public string Guid { get; set; }

        [Url]
        [Required]
        [Display(Name = "Original url")]
        public string LongUrl { get; set; }

        [Url]
        [Display(Name = "Short url")]
        public string ShortUrl { get; set; }

        private int _redirectCount;
        [Display(Name = "Redirect count")]
        public int RedirectCount
        {
            get
            {
                return _redirectCount;
            }
            set
            {
                if (value < 0)
                {
                    return;
                }

                _redirectCount = value;
            }
        }

        [Display(Name = "Creation date")]
        public DateTime CreationDate { get; set; }
    }

    public class UrlEntryContext : DbContext, IUrlEntriesContext
    {
        public UrlEntryContext(DbContextOptions<UrlEntryContext> options)
            : base(options)
        { }

        public DbSet<UrlEntry> UrlEntries { get; set; }
        
        public EntityEntry<UrlEntry> Add(UrlEntry urlEnrty) => base.Add(urlEnrty);

        public EntityEntry<UrlEntry> Update(UrlEntry urlEntry) => base.Update(urlEntry);

        public Task<int> SaveChangesAsync() => base.SaveChangesAsync();
    }
}
