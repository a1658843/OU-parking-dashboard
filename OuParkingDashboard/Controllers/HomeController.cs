using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OuParkingDashboard.Data;
using OuParkingDashboard.Models.ViewModels;

namespace OuParkingDashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /
        public async Task<IActionResult> Index()
        {
            var garages = await _db.Garages
                .Include(g => g.Status)
                .Select(g => new GarageViewModel
                {
                    Name = g.Name,
                    Capacity = g.Capacity,
                    Available = g.Status != null ? g.Status.Available : 0
                })
                .ToListAsync();

            return View(garages);
        }
    }
}
