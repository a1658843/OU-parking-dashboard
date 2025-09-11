using System;

namespace OuStudentPortal.Models.Parking
{
    public class GarageStatusHistory
    {
        public int Id { get; set; }
        public int GarageId { get; set; }

        // Indicates if the garage is available (true = open spots, false = full)
        public int Available { get; set; }

        // UTC timestamp for when the status was recorded
        public DateTime TimestampUtc { get; set; }

        // Navigation property
        public Garage Garage { get; set; } = null!;
    }
}
