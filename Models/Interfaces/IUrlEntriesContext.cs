using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading.Tasks;

namespace UrlCompressorService.Models.Interfaces
{
    public interface IUrlEntriesContext
    {
        DbSet<UrlEntry> UrlEntries { get; set; }

        EntityEntry<UrlEntry> Add(UrlEntry urlEnrty);

        Task<int> SaveChangesAsync();

        EntityEntry<UrlEntry> Update(UrlEntry urlEntry);
    }
}
