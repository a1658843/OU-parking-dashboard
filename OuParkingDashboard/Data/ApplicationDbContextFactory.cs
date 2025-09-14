using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using OuParkingDashboard.Data;

namespace OuParkingDashboard.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // Use your actual connection string here
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=OuParkingDashboardDb;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
