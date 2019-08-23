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
    public class UploadController : ControllerBase
    {
        public readonly ApplicationDbContext _context;

        public UploadController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Upload>>> GetUploads()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            userId = "ce693acd-d08d-4303-9e5d-a26b832abc38";

            var uploads = await _context.Uploads.Where(x => x.UserId == userId).Include(x => x.User).ToListAsync();

            return uploads;
        }

        [HttpGet("{UploadId}")]
        public async Task<ActionResult<Upload>> GetUploadById(int uploadId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            userId = "ce693acd-d08d-4303-9e5d-a26b832abc38";

            var upload = await _context.Uploads.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == userId && x.UploadId == uploadId);

            if (upload == null)
            {
                return NoContent();
            }

            return upload;
        }

        [HttpPost]
        public async Task<ActionResult<Upload>> AddUpload(Upload upload)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            userId = "ce693acd-d08d-4303-9e5d-a26b832abc38";

            var newUpload = new Upload();

            newUpload.ImageUrl = upload.ImageUrl;
            newUpload.UploadDate = upload.UploadDate;
            newUpload.UploadDescription = upload.UploadDescription;
            newUpload.Price = upload.Price;
            newUpload.UserId = userId;

            await _context.Uploads.AddAsync(newUpload);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{UploadId}")]
        public async Task<ActionResult<Upload>> EditUpload(int uploadId, Upload upload)
        {
            if (uploadId != upload.UploadId)
            {
                return BadRequest();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            userId = "ce693acd-d08d-4303-9e5d-a26b832abc38";
            upload.UserId = userId;
            
            _context.Entry(upload).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{UploadId}")]
        public async Task<ActionResult<Upload>> DeleteUpload(int uploadId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            userId = "ce693acd-d08d-4303-9e5d-a26b832abc38";

            var upload = await _context.Uploads.FirstOrDefaultAsync(x => x.UserId == userId && x.UploadId == uploadId);

            if (upload == null)
            {
                return NoContent();
            }

            _context.Uploads.Remove(upload);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}