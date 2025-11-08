using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UrlShortenerService.MVC.Data.Entities.Identities;


namespace UrlShortenerService.MVC.Data
{
    public class AppIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ✅ Customize ApplicationUser fields
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.FullName)
                      .HasMaxLength(150);        // Adjust length as needed

                entity.Property(u => u.Address)
                      .HasMaxLength(300);        // Adjust length as needed
            });
        }
    }
}
