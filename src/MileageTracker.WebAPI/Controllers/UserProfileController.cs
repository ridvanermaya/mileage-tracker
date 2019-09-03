using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MileageTracker.WebAPI.Models;

namespace MileageTracker.WebAPI.Controllers
{
    public class UserProfileController : MTBaseController
    {
        public readonly ApplicationDbContext _context;

        public UserProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<UserProfile>> GetUserProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.Name); 
            var userProfile = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == userId);

            if (userProfile == null)
            {
                return NotFound();
            }

            return userProfile;
        }

        [HttpGet("{UserProfileId}")]
        public async Task<ActionResult<UserProfile>> GetUserProfileById(int userProfileId)
        {
            var userId = User.FindFirstValue(ClaimTypes.Name); 
            var userProfile = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == userId && x.UserProfileId == userProfileId);

            if (userProfile == null)
            {
                return NotFound();
            }

            return userProfile;
        }

        [HttpPut("{UserProfileId}")]
        public async Task<ActionResult> EditUserProfile(int userProfileId, UserProfile userProfile)
        {
            if (userProfileId != userProfile.UserProfileId)
            {
                return BadRequest();
            }

            var userId = User.FindFirstValue(ClaimTypes.Name); 

            _context.Entry(userProfile).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<UserProfile>> AddUserProfile(UserProfile userProfile)
        {
            var userId = User.FindFirstValue(ClaimTypes.Name);
            var address = await _context.Addresses.FirstOrDefaultAsync(x => x.UserId == userId);
            var newUserProfile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == userId);

            if (newUserProfile != null)
            {
                return NoContent();
            }

            newUserProfile = new UserProfile();

            newUserProfile.FirstName = userProfile.FirstName;
            newUserProfile.LastName = userProfile.LastName;
            newUserProfile.PhoneNumber = userProfile.PhoneNumber;
            newUserProfile.AddressId = address.AddressId;
            newUserProfile.UserId = userId;

            await _context.UserProfiles.AddAsync(newUserProfile);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}