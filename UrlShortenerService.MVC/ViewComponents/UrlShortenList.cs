using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using UrlShortenerService.MVC.Data;

namespace UrlShortenerService.MVC.ViewComponents
{
    public class UrlShortenListViewComponent : ViewComponent
    {
        private readonly AppDbContext _db;

        public UrlShortenListViewComponent(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? top = null)
        {
            // Query simplified: EF infers the type automatically (no casting issues)
            var query = _db.ShortUrls
                .AsNoTracking()
                .OrderByDescending(s => s.CreatedAt);
            var allItems = await query.ToListAsync();
            return View(allItems);


        }
    }
}
