using Microsoft.EntityFrameworkCore;
using OuStudentPortal.Models.Parking;

namespace OuStudentPortal.Data
{
    public static class ParkingSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await db.Database.MigrateAsync();

            if (!await db.Garages.AnyAsync())
            {
                var garages = new[]
                {
                    new Garage { Code = "GAR_A", Name = "Garage A", Capacity = 800, Latitude = 35.208, Longitude = -97.445 },
                    new Garage { Code = "GAR_B", Name = "Garage B", Capacity = 1200, Latitude = 35.206, Longitude = -97.442 },
                    new Garage { Code = "GAR_C", Name = "Garage C", Capacity = 600, Latitude = 35.205, Longitude = -97.448 }
                };
                db.Garages.AddRange(garages);
                await db.SaveChangesAsync();

                foreach (var g in garages)
                {
                    db.GarageStatuses.Add(new GarageStatus
                    {
                        GarageId = g.Id,
                        Available = g.Capacity,
                        UpdatedAtUtc = DateTime.UtcNow
                    });
                }
                await db.SaveChangesAsync();
            }
        }
    }
}
