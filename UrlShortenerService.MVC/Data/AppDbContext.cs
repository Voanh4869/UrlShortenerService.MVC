using Microsoft.EntityFrameworkCore;
using UrlShortenerService.MVC.Data.Entities;
using System;

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

            modelBuilder.Entity<ShortUrl>().ToTable("ShortUrls");

            modelBuilder.Entity<ShortUrl>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.OriginalUrl)
                      .HasMaxLength(2048)
                      .IsRequired();

                entity.Property(e => e.ShortCode)
                      .HasMaxLength(50);
                      //.IsRequired();

                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()")
                      .IsRequired();
            });

            // Sample seed data
            //modelBuilder.Entity<ShortUrl>().HasData(
            //    new ShortUrl
            //    {
            //        Id = 1,
            //        OriginalUrl = "https://www.microsoft.com",
            //        ShortCode = "msft2025",
            //        CreatedAt = new DateTime(2025, 11, 6, 7, 44, 28, DateTimeKind.Utc)
            //    },
            //    new ShortUrl
            //    {
            //        Id = 2,
            //        OriginalUrl = "https://www.github.com",
            //        ShortCode = "gh2025",
            //        CreatedAt = new DateTime(2025, 11, 6, 7, 44, 29, DateTimeKind.Utc)
            //    }
            //);
        }
    }
}
