using OuParkingDashboard.Data;
using OuParkingDashboard.Services;
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

// ✅ Enable serving static files from wwwroot (needed for PDFs, CSS, JS, etc.)
app.UseStaticFiles();

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
        body { font-family: Arial, sans-serif; background: #f0f0f0; text-align: center; padding: 40px; }
        h1, h2 { margin-bottom: 20px; }
        table { margin: auto; border-collapse: collapse; width: 80%; background: #fff; box-shadow: 0 2px 5px rgba(0,0,0,0.1); }
        th, td { padding: 12px; border: 1px solid #ddd; text-align: center; }
        th { background-color: #eee; }
        .bar-container { width: 100%; background-color: #ddd; border-radius: 5px; overflow: hidden; position: relative; height: 25px; }
        .bar { height: 100%; text-align: center; line-height: 25px; color: white; font-weight: bold; }
        .gray { background-color: #999; }
        .green { background-color: #4CAF50; }
        .yellow { background-color: #FFC107; }
        .orange { background-color: #FF9800; }
        .red { background-color: #F44336; }
        .bar span { position: absolute; width: 100%; left: 0; top: 0; text-align: center; color: white; }
        section { margin-top: 40px; }
        a.pdf-link { font-size: 18px; font-weight: bold; color: #004080; text-decoration: none; }
        a.pdf-link:hover { text-decoration: underline; }
    </style>
</head>
<body>
    <h1>🚗 Live Garage Status</h1>

    <table id='garageTable'>
        <tr><th>Garage</th><th>Capacity</th><th>Available</th><th>Fullness</th></tr>
    </table>

    <section id='campus-map'>
        <h2>🗺️ Campus Map</h2>
        <p>
            <a class='pdf-link' href='/pdfs/ou-parking-map-2025.pdf' target='_blank' rel='noopener noreferrer'>
                📄 View Full Parking Map (PDF)
            </a>
        </p>
    </section>

    <script>
        function getBarColor(percentage) {
            if (percentage === 0) return 'gray';
            if (percentage <= 50) return 'green';
            else if (percentage <= 70) return 'yellow';
            else if (percentage <= 90) return 'orange';
            else return 'red';
        }

        async function fetchGarageData() {
            try {
                const response = await fetch('/api/garages');
                const garages = await response.json();
                const table = document.getElementById('garageTable');

                // Clear all rows except header
                table.innerHTML = '<tr><th>Garage</th><th>Capacity</th><th>Available</th><th>Fullness</th></tr>';

                garages.forEach(g => {
                    const row = table.insertRow();
                    const percentFull = Math.floor((1 - g.available / g.capacity) * 100); // floor for realistic % 

                    row.insertCell(0).innerText = g.name;
                    row.insertCell(1).innerText = g.capacity;
                    row.insertCell(2).innerText = g.available;

                    const percentCell = row.insertCell(3);
                    const color = getBarColor(percentFull);
                    percentCell.innerHTML = `
                        <div class='bar-container'>
                            <div class='bar ${color}' style='width:${percentFull}%;'>
                                <span>${percentFull}%</span>
                            </div>
                        </div>`;
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

// Seed data on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<ApplicationDbContext>();

    var seeder = new ParkingSeeder(db);
    await seeder.SeedAsync();
}

// Map controllers
app.MapControllers();

app.Run();
