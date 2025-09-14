using System.ComponentModel.DataAnnotations;

namespace OuParkingDashboard.Models.Parking
{
    public class Garage
    {
        public int Id { get; set; }

        [Required, MaxLength(16)]
        public string Code { get; set; } = string.Empty; //  "GAR_A"

        [Required, MaxLength(64)]
        public string Name { get; set; } = string.Empty; //  "Garage A"

        [Range(0, 10000)]
        public int Capacity { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // 1:1 
        public GarageStatus? Status { get; set; }

        public List<GarageStatusHistory> StatusHistories { get; set; } = new();

    }
}
