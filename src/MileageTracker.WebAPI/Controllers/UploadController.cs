using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MileageTracker.WebAPI.Models;

namespace MileageTracker.WebAPI.Controllers
{
    public class UploadController : MTBaseController
    {
        public readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _environment;

        public UploadController(ApplicationDbContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Upload>>> GetUploads()
        {
            var userId = User.FindFirstValue(ClaimTypes.Name);

            var uploads = await _context.Uploads.Where(x => x.UserId == userId).Include(x => x.User).ToListAsync();

            return uploads;
        }

        [HttpGet("{UploadId}")]
        public async Task<ActionResult<Upload>> GetUploadById(int uploadId)
        {
            var userId = User.FindFirstValue(ClaimTypes.Name);

            var upload = await _context.Uploads.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == userId && x.UploadId == uploadId);

            if (upload == null)
            {
                return NoContent();
            }

            return upload;
        }

        [HttpPost]
        public async Task<ActionResult<Upload>> AddUpload(IFormFile file, [FromForm]string description, [FromForm]double price)
        {
            var userId = User.FindFirstValue(ClaimTypes.Name);

            var directoryInfo = new DirectoryInfo(Path.Combine(_environment.ContentRootPath, "files"));
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            var fileName = $"{Guid.NewGuid()}-{file.FileName}";

            using (var fileStream = new FileStream(Path.Combine(directoryInfo.FullName, fileName), FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var newUpload = new Upload();

            newUpload.ImageUrl = fileName;
            newUpload.UploadDate = DateTime.Now;
            newUpload.UploadDescription = description;
            newUpload.Price = price;
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

            var userId = User.FindFirstValue(ClaimTypes.Name);
            upload.UserId = userId;

            _context.Entry(upload).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{UploadId}")]
        public async Task<ActionResult<Upload>> DeleteUpload(int uploadId)
        {
            var userId = User.FindFirstValue(ClaimTypes.Name);

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