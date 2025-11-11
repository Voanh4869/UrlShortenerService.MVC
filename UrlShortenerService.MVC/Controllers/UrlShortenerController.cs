using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UrlShortenerService.MVC.Data;
using UrlShortenerService.MVC.Data.Entities;
using UrlShortenerService.MVC.ViewModels;

namespace UrlShortenerService.MVC.Controllers
{
    //[Route("UrlShortener")]
    [Authorize]
    public class UrlShortenerController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public UrlShortenerController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // ✅ GET /UrlShortener
        [HttpGet("")]
        public IActionResult Index()
        {
            return View("~/Views/Home/Index.cshtml");
        }

        // ✅ POST /UrlShortener/Shorten
        [HttpPost("Shorten")]
        public async Task<IActionResult> Shorten(string originalUrl)
        {
            var vm = new QrCodeVM();

            // Validation
            if (string.IsNullOrWhiteSpace(originalUrl) ||
                !Uri.IsWellFormedUriString(originalUrl, UriKind.Absolute))
            {
                vm.ErrorMessage = "Please enter a valid absolute URL.";
                return View("~/Views/Home/Index.cshtml", vm);
            }

            // Generate short code
            string shortCode = GenerateShortCode();
            while (_db.ShortUrls.Any(s => s.ShortCode == shortCode))
            {
                shortCode = GenerateShortCode();
            }

            // Build short URL
            // var host = " https://roiliest-troublingly-vincenza.ngrok-free.dev"; // domain chính dùng để chạy online
             var host = $"{Request.Scheme}://{Request.Host}"; // dùng để chạy local
            string shortUrl = $"{host}/{shortCode}";

            var entity = new ShortUrl
            {
                OriginalUrl = originalUrl,
                ShortCode = shortCode,
                CreatedAt = DateTime.UtcNow,
                ShortLink = shortUrl,
            };

            _db.ShortUrls.Add(entity);
            await _db.SaveChangesAsync();

          
            // ✅ Generate QR Code for short URL
            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(shortUrl, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new QRCode(qrData);
            using var qrBitmap = qrCode.GetGraphic(20);

            using var stream = new MemoryStream();
            qrBitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            var qrBytes = stream.ToArray();

            // Convert to Base64 string for display
            string base64 = Convert.ToBase64String(qrBytes);

            // Save file in wwwroot/qrcodes/
            string qrFolder = Path.Combine(_env.WebRootPath, "qrcodes");
            if (!Directory.Exists(qrFolder))
                Directory.CreateDirectory(qrFolder);

            string fileName = $"qr_{DateTime.Now:yyyyMMddHHmmssfff}.png";
            string filePath = Path.Combine(qrFolder, fileName);
            await System.IO.File.WriteAllBytesAsync(filePath, qrBytes);

            // ✅ Populate ViewModel
            vm.Url = shortUrl;
            vm.QrImageBase64 = $"data:image/png;base64,{base64}";
            vm.SavedFilePath = $"/qrcodes/{fileName}";
            vm.ErrorMessage = null;

            // ✅ Show Result.cshtml in Home folder
            return View("~/Views/Home/Result.cshtml", vm);
        }

        [AllowAnonymous]
        [HttpGet("{shortCode}")]
        public IActionResult RedirectToOriginal(string shortCode)
        {
            if (string.IsNullOrWhiteSpace(shortCode))
                return NotFound();

            var entry = _db.ShortUrls.FirstOrDefault(s => s.ShortCode == shortCode);
            if (entry == null)
                return NotFound();

            return Redirect(entry.OriginalUrl);
        }

        // ✅ Helper: generate unique short code
        private static string GenerateShortCode()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8);
        }

        // ✅ POST /UrlShortener/Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.ShortUrls.FindAsync(id);
            if (item == null)
                return NotFound();

            _db.ShortUrls.Remove(item);
            await _db.SaveChangesAsync();

            // Return to main page
            return View("~/Views/Home/Index.cshtml");
        }
    }
}
