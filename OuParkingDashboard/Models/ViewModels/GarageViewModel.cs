using System;

namespace OuParkingDashboard.Models.ViewModels
{
    public class GarageViewModel
    {
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int Available { get; set; }

        public int PercentFull
        {
            get
            {
                if (Capacity == 0) return 0;
                var pct = (1.0 - (double)Available / Capacity) * 100.0;
                return (int)Math.Floor(pct);
            }
        }
    }
}
