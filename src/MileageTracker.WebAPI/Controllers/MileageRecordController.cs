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

        [HttpGet("{MileageRecordId}")]
        public async Task<ActionResult<MileageRecord>> GetMileageRecord(int mileageRecordId)
        {
            var mileageRecord = await _context.MileageRecords.FirstOrDefaultAsync(x => x.MileageRecordId == mileageRecordId);

            if (mileageRecord == null)
            {
                return NotFound();
            }
            return mileageRecord;
        }

        [HttpPost]
        public async Task<ActionResult<MileageRecord>> PostMileageRecord(MileageRecord mileageRecord)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            MileageRecord newMileageRecord = new MileageRecord();
            
            newMileageRecord.Service = mileageRecord.Service;
            newMileageRecord.Mileage = mileageRecord.Mileage;
            newMileageRecord.StartDateTime = mileageRecord.StartDateTime;
            newMileageRecord.EndDateTime = mileageRecord.EndDateTime;
            newMileageRecord.UserId = userId;

            _context.MileageRecords.Add(newMileageRecord);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{MileageRecordId}")]
        public async Task<ActionResult> DeleteMileageRecord(int mileageRecordId)
        {
            var foundMileageRecord = await _context.MileageRecords.FirstOrDefaultAsync(x => x.MileageRecordId == mileageRecordId);

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