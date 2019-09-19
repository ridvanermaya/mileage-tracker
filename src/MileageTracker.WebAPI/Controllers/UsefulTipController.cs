using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MileageTracker.WebAPI.Models;

namespace MileageTracker.WebAPI.Controllers
{
    public class UsefulTipController : MTBaseController
    {
        public readonly ApplicationDbContext _context;

        public UsefulTipController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsefulTip>>> GetUsefulTips()
        {
            var userId = User.FindFirstValue(ClaimTypes.Name);

            var usefulTips = await _context.UsefulTips.ToListAsync();

            return usefulTips;
        }
    }
}