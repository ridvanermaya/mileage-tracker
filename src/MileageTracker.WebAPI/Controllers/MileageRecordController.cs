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
    public class MileageRecordController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MileageRecordController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MileageRecord>>> GetMileageRecords()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var mileageRecords = await _context.MileageRecords.Where(x => x.UserId == userId).ToListAsync();

            return mileageRecords;
        }
    }
}