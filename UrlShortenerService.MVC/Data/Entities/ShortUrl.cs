using System;
using System.ComponentModel.DataAnnotations;

namespace UrlShortenerService.MVC.Data.Entities
{
    public class ShortUrl
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Url]
        public string OriginalUrl { get; set; } = string.Empty;

        [Required]
        public string ShortCode { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
