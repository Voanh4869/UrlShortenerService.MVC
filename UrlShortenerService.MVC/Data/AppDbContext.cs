using Microsoft.EntityFrameworkCore;
using UrlShortenerService.MVC.Data.Entities;
using System;
using UrlShortenerService.MVC.Common;

namespace UrlShortenerService.MVC.Data
{
    public class AppDbContext : DbContext
    {
        //=== Constructor ===//
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        //=== DbSet for each entity (database tables) ===//
        public DbSet<ShortUrl> ShortUrls { get; set; }

        //=== Configure connection if not already configured (safety net) ===//
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Fallback connection (if Program.cs configuration is missing)
                // optionsBuilder.UseSqlServer("Server=.;Database=UrlShortenerDB;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        //=== Fluent API configuration (table, constraints, seed data) ===//
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ràng buộc độ dài cho cột Name
            modelBuilder.Entity<ShortUrl>()
                .Property(p => p.OriginalUrl)
                .HasMaxLength(MaxLengthConfig.ORIGIN_URL)
                .IsRequired();

            modelBuilder.Entity<ShortUrl>()
                .Property(p => p.ShortCode)
                .HasMaxLength(MaxLengthConfig.SHORTENED_URL);

            modelBuilder.Entity<ShortUrl>()
                .Property(p => p.CreatedAt)
                .HasColumnType("datetime");

        }

    }
}

