using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OuParkingDashboard.Data;
using OuParkingDashboard.Models.Parking;


namespace OuParkingDashboard.Controllers
{
    [ApiController]
    [Route("api/parking")]
    public class ParkingApiController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public ParkingApiController(ApplicationDbContext db) => _db = db;

        // DTOs（简洁返回，不直接暴露实体）
        public record GarageDto(int Id, string Code, string Name, int Capacity, int Available, DateTime UpdatedAtUtc, double? Lat, double? Lng);
        public record UpdateStatusDto(int Available);
        public record HistoryPointDto(DateTime TimestampUtc, int Available);

        [HttpGet("garages")]
        public async Task<ActionResult<IEnumerable<GarageDto>>> GetGarages()
        {
            var list = await _db.Garages
                .Include(g => g.Status)
                .Select(g => new GarageDto(
                    g.Id, g.Code, g.Name, g.Capacity,
                    g.Status!.Available, g.Status.UpdatedAtUtc,
                    g.Latitude, g.Longitude))
                .ToListAsync();

            return Ok(list);
        }

        [HttpGet("garages/{id:int}")]
        public async Task<ActionResult<GarageDto>> GetGarage(int id)
        {
            var g = await _db.Garages.Include(x => x.Status).FirstOrDefaultAsync(x => x.Id == id);
            if (g is null || g.Status is null) return NotFound();

            return Ok(new GarageDto(
                g.Id, g.Code, g.Name, g.Capacity,
                g.Status.Available, g.Status.UpdatedAtUtc,
                g.Latitude, g.Longitude));
        }

        // 手动更新某车库状态（演示/测试用）
        [HttpPut("garages/{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
        {
            var g = await _db.Garages.Include(x => x.Status).FirstOrDefaultAsync(x => x.Id == id);
            if (g is null || g.Status is null) return NotFound();

            g.Status.Available = Math.Clamp(dto.Available, 0, g.Capacity);
            g.Status.UpdatedAtUtc = DateTime.UtcNow;

            _db.GarageStatusHistories.Add(new Models.Parking.GarageStatusHistory
            {
                GarageId = g.Id,
                Available = g.Status.Available,
                TimestampUtc = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("garages/{id:int}/history")]
        public async Task<ActionResult<IEnumerable<HistoryPointDto>>> History(int id, [FromQuery] int minutes = 60)
        {
            var since = DateTime.UtcNow.AddMinutes(-minutes);
            var points = await _db.GarageStatusHistories
                .Where(h => h.GarageId == id && h.TimestampUtc >= since)
                .OrderBy(h => h.TimestampUtc)
                .Select(h => new HistoryPointDto(h.TimestampUtc, h.Available))
                .ToListAsync();

            return Ok(points);
        }
    }
}
