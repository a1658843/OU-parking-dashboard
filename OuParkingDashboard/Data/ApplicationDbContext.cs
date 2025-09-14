using Microsoft.EntityFrameworkCore;
using OuParkingDashboard.Models;
using OuParkingDashboard.Models.Parking;

namespace OuParkingDashboard.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Students
        public DbSet<Student> Students { get; set; } = null!;

        // Parking
        public DbSet<Garage> Garages { get; set; } = null!;
        public DbSet<GarageStatus> GarageStatuses { get; set; } = null!;
        public DbSet<GarageStatusHistory> GarageStatusHistories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Garage has unique Code
            modelBuilder.Entity<Garage>()
                .HasIndex(g => g.Code)
                .IsUnique();

            // Garage → GarageStatus (1:1)
            modelBuilder.Entity<Garage>()
                .HasOne(g => g.Status)
                .WithOne(s => s.Garage)
                .HasForeignKey<GarageStatus>(s => s.GarageId)
                .OnDelete(DeleteBehavior.Cascade);

            // GarageStatusHistory → Garage (many-to-1)
            modelBuilder.Entity<GarageStatusHistory>(entity =>
            {
                entity.HasKey(h => h.Id);

                entity.HasOne(h => h.Garage)
                      .WithMany(g => g.StatusHistories)
                      .HasForeignKey(h => h.GarageId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
