using Microsoft.EntityFrameworkCore;
using UrlShortenerService.MVC.Data;

var builder = WebApplication.CreateBuilder(args);

// === Database Connection === //
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UrlShortenerConnection")));

// === MVC === //
builder.Services.AddControllersWithViews();

var app = builder.Build();

// === Middleware === //
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Map a root-level route for short codes (e.g. "/8d04ed2b")
app.MapControllerRoute(
    name: "short",
    pattern: "{shortCode}",
    defaults: new { controller = "UrlShortener", action = "RedirectToOriginal" });

// Explicit route for the UrlShortener controller (optional but clearer)
app.MapControllerRoute(
    name: "urlshortener",
    pattern: "UrlShortener/{action=Index}/{id?}",
    defaults: new { controller = "UrlShortener" });

// default MVC route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
