using OuParkingDashboard.Data;
using OuParkingDashboard.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register hosted service and seeder
builder.Services.AddHostedService<MockParkingFeedService>();
builder.Services.AddTransient<ParkingSeeder>();

// Enable CORS for development
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("DevAllowAll", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// Add controllers + views (MVC)
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseCors("DevAllowAll");

// Enable static files from wwwroot (PDFs, css, js)
app.UseStaticFiles();

// Map a route for the old prototype HTML (kept for reference)
// This re-uses your existing hard-coded HTML but at /prototype so nothing is deleted.
app.MapGet("/prototype", () =>
{
    // copy-paste of your old HTML page (kept as-is). Minimal change: route is /prototype.
    var html = @"<!DOCTYPE html><html lang='en'><head> ... </head><body> ... </body></html>";
    // NOTE: paste the *exact* HTML you previously had here (or leave it as a short message).
    // For brevity in this snippet I collapsed it; in your local Program.cs paste the original content.
    return Results.Content(html, "text/html");
});

// API endpoint to get garage data as JSON (keep this as-is)
app.MapGet("/api/garages", async (ApplicationDbContext db) =>
{
    var garages = await db.Garages
                          .Include(g => g.Status)
                          .Select(g => new
                          {
                              g.Name,
                              g.Capacity,
                              Available = g.Status != null ? g.Status.Available : 0
                          })
                          .ToListAsync();
    return Results.Json(garages);
});

// MVC default route - this sends "/" to HomeController.Index
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

// Seed data on startup using DI (ParkingSeeder)
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<ParkingSeeder>();
    await seeder.SeedAsync();
}

// Map other controllers if you have attribute routes
app.MapControllers();

app.Run();
