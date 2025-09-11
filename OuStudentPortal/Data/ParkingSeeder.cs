using OuStudentPortal.Data;
using OuStudentPortal.Models.Parking;
using Microsoft.EntityFrameworkCore;

namespace OuStudentPortal.Services
{
    public class ParkingSeeder
    {
        private readonly ApplicationDbContext _db;

        public ParkingSeeder(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task SeedAsync()
        {
            if (await _db.Garages.AnyAsync())
                return; // Already seeded

            var garages = new List<Garage>
            {
                new Garage
                {
                    Code = "GAR_A",
                    Name = "Garage A",
                    Capacity = 1200,
                    Status = new GarageStatus
                    {
                        Available = 4, // almost full
                        UpdatedAtUtc = DateTime.UtcNow
                    }
                },
                new Garage
                {
                    Code = "GAR_B",
                    Name = "Garage B",
                    Capacity = 1500,
                    Status = new GarageStatus
                    {
                        Available = 750, // ~50% full
                        UpdatedAtUtc = DateTime.UtcNow
                    }
                },
                new Garage
                {
                    Code = "GAR_C",
                    Name = "Garage C",
                    Capacity = 2000,
                    Status = new GarageStatus
                    {
                        Available = 400, // ~80% full
                        UpdatedAtUtc = DateTime.UtcNow
                    }
                },
                new Garage
                {
                    Code = "GAR_D",
                    Name = "Garage D",
                    Capacity = 1000,
                    Status = new GarageStatus
                    {
                        Available = 0, // full
                        UpdatedAtUtc = DateTime.UtcNow
                    }
                }
            };

            await _db.Garages.AddRangeAsync(garages);
            await _db.SaveChangesAsync();
        }
    }
}
