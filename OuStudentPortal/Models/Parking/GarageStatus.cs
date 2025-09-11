namespace OuStudentPortal.Models.Parking
{
    public class GarageStatus
    {
        public int Id { get; set; }
        public int GarageId { get; set; }
        public int Available { get; set; }
        public DateTime UpdatedAtUtc { get; set; }

        public Garage Garage { get; set; } = null!;
    }
}
