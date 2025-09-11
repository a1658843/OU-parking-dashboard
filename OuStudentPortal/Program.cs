using OuStudentPortal.Data;
using OuStudentPortal.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register hosted service
builder.Services.AddHostedService<MockParkingFeedService>();

// Enable CORS for development
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("DevAllowAll", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// Add controllers (API endpoints)
builder.Services.AddControllers();

var app = builder.Build();

app.UseCors("DevAllowAll");

// Root dashboard page
app.MapGet("/", () =>
{
    var html = @"<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Garage Status Dashboard</title>
    <style>
        body { font-family: Arial, sans-serif; background: #f0f0f0; text-align: center; padding: 50px; }
        table { margin: auto; border-collapse: collapse; width: 60%; }
        th, td { padding: 10px; border: 1px solid #ccc; }
        th { background-color: #eee; }
    </style>
</head>
<body>
    <h1>🚗 Live Garage Status</h1>
    <table id='garageTable'>
        <tr><th>Garage</th><th>Capacity</th><th>Available</th></tr>
    </table>

    <script>
        async function fetchGarageData() {
            try {
                const response = await fetch('/api/garages');
                const garages = await response.json();
                const table = document.getElementById('garageTable');

                // Clear all rows except header
                table.innerHTML = '<tr><th>Garage</th><th>Capacity</th><th>Available</th></tr>';

                garages.forEach(g => {
                    const row = table.insertRow();
                    row.insertCell(0).innerText = g.name;
                    row.insertCell(1).innerText = g.capacity;
                    row.insertCell(2).innerText = g.available;
                });
            } catch (err) {
                console.error('Error fetching garage data:', err);
            }
        }

        // Initial fetch
        fetchGarageData();

        // Refresh every 5 seconds
        setInterval(fetchGarageData, 5000);
    </script>
</body>
</html>";

    return Results.Content(html, "text/html");
});

// API endpoint to get garage data as JSON
app.MapGet("/api/garages", async (ApplicationDbContext db) =>
{
    var garages = await db.Garages
                          .Include(g => g.Status) // assuming navigation property to GarageStatus
                          .Select(g => new
                          {
                              g.Name,
                              g.Capacity,
                              Available = g.Status != null ? g.Status.Available : 0
                          })
                          .ToListAsync();
    return Results.Json(garages);
});

// Seed data on startup
await ParkingSeeder.SeedAsync(app.Services);

// Map controllers
app.MapControllers();

app.Run();
