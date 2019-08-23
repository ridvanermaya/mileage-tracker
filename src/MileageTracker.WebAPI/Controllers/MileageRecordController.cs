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
            userId = "edeba505-384c-4c3d-95cc-1872ebb73910";
            var mileageRecords = await _context.MileageRecords.Where(x => x.UserId == userId).Include(x => x.User).ToListAsync();

            return mileageRecords;
        }

        [HttpGet("{MileageRecordId}")]
        public async Task<ActionResult<MileageRecord>> GetMileageRecordById(int mileageRecordId)
        {
            var mileageRecord = await _context.MileageRecords.Include(x => x.User).FirstOrDefaultAsync(x => x.MileageRecordId == mileageRecordId);

            if (mileageRecord == null)
            {
                return NotFound();
            }
            return mileageRecord;
        }

        [HttpPost]
        public async Task<ActionResult<MileageRecord>> AddMileageRecord(MileageRecord mileageRecord)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            userId = "edeba505-384c-4c3d-95cc-1872ebb73910";

            MileageRecord newMileageRecord = new MileageRecord();
            
            newMileageRecord.Service = mileageRecord.Service.ToUpper();
            newMileageRecord.Mileage = mileageRecord.Mileage;
            newMileageRecord.StartDateTime = mileageRecord.StartDateTime;
            newMileageRecord.EndDateTime = mileageRecord.EndDateTime;
            newMileageRecord.UserId = userId;

            await _context.MileageRecords.AddAsync(newMileageRecord);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{MileageRecordId}")]
        public async Task<ActionResult> DeleteMileageRecord(int mileageRecordId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            userId = "edeba505-384c-4c3d-95cc-1872ebb73910";
            var foundMileageRecord = await _context.MileageRecords.FirstOrDefaultAsync(x => x.MileageRecordId == mileageRecordId && x.UserId == userId);

            if (foundMileageRecord == null)
            {
                return NotFound();
            }

            _context.MileageRecords.Remove(foundMileageRecord);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}