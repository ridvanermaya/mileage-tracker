using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MileageTracker.WebAPI.Models;

namespace MileageTracker.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceController : ControllerBase
    {
        public readonly ApplicationDbContext _context;

        public ServiceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Service>>> GetServices()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            userId = "ce693acd-d08d-4303-9e5d-a26b832abc38";

            var services = await _context.Services.Where(x => x.UserId == userId).Include(x => x.User).ToListAsync();

            return services;
        }

        [HttpGet("{ServiceId}")]
        public async Task<ActionResult<Service>> GetServiceById(int serviceId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            userId = "ce693acd-d08d-4303-9e5d-a26b832abc38";

            var service = await _context.Services.Include(x => x.User).FirstOrDefaultAsync(x => x.ServiceId == serviceId && x.UserId == userId);

            if (service == null)
            {
                return NotFound();
            }

            return service;
        }

        [HttpPost]
        public async Task<ActionResult<Service>> AddService(Service service)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            userId = "ce693acd-d08d-4303-9e5d-a26b832abc38";

            var newService = await _context.Services.FirstOrDefaultAsync(x => x.Name == service.Name && x.UserId == userId);

            if (newService != null)
            {
                return NoContent();
            }

            newService = new Service();

            newService.Name = service.Name;
            newService.UserId = userId;

            await _context.Services.AddAsync(newService);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{ServiceId}")]
        public async Task<ActionResult<Service>> DeleteService(int serviceId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            userId = "ce693acd-d08d-4303-9e5d-a26b832abc38";

            var service = await _context.Services.FirstOrDefaultAsync(x => x.ServiceId == serviceId && x.UserId == userId);

            if (service == null)
            {
                return NotFound();
            }

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}