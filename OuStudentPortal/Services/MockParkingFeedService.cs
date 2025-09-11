using Microsoft.EntityFrameworkCore;
using OuStudentPortal.Data;
using OuStudentPortal.Models.Parking;


namespace OuStudentPortal.Services
{
    public class MockParkingFeedService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly Random _rand = new();

        public MockParkingFeedService(IServiceProvider services) => _services = services;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var statuses = await db.GarageStatuses.ToListAsync(stoppingToken);
                foreach (var s in statuses)
                {
                    var garage = await db.Garages.FindAsync(new object[] { s.GarageId }, stoppingToken);
                    if (garage is null) continue;

                    // -5 到 +5 的随机波动
                    var delta = _rand.Next(-5, 6);
                    var next = Math.Clamp(s.Available + delta, 0, garage.Capacity);

                    s.Available = next;
                    s.UpdatedAtUtc = DateTime.UtcNow;

                    db.GarageStatusHistories.Add(new Models.Parking.GarageStatusHistory
                    {
                        GarageId = s.GarageId,
                        Available = next,
                        TimestampUtc = DateTime.UtcNow
                    });
                }

                await db.SaveChangesAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
