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
    public class AddressController : ControllerBase
    {
        public readonly ApplicationDbContext _context;

        public AddressController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Address>> GetAddress()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            userId = "ce693acd-d08d-4303-9e5d-a26b832abc38";

            var address = await _context.Addresses.Include(x => x.User).FirstOrDefaultAsync();

            if (userId == null)
            {
                return Unauthorized();
            }

            return address;
        }

        [HttpGet("{AddressId}")]
        public async Task<ActionResult<Address>> GetAddressById(int addressId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            userId = "ce693acd-d08d-4303-9e5d-a26b832abc38";

            var address = await _context.Addresses.Include(x => x.User).FirstOrDefaultAsync(x => x.AddressId == addressId && x.UserId == userId);

            if (address == null)
            {
                return NotFound();
            }

            return address;
        }

        [HttpPut("{AddressId}")]
        public async Task<ActionResult> EditAddress(int addressId, Address address)
        {
            if (addressId != address.AddressId)
            {
                return BadRequest();
            }

            _context.Entry(address).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Address>> AddAddress(Address address)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            userId = "ce693acd-d08d-4303-9e5d-a26b832abc38";
            var newAddress = await _context.Addresses.FirstOrDefaultAsync(x => x.UserId == userId);

            if (newAddress != null)
            {
                return NoContent();
            }

            newAddress = new Address();

            newAddress.AddressLineOne = address.AddressLineOne;
            newAddress.AddressLineTwo = address.AddressLineTwo;
            newAddress.City = address.City;
            newAddress.StateAbbreviation = address.StateAbbreviation.ToUpper();
            newAddress.ZipCode = address.ZipCode;
            newAddress.UserId = userId;

            await _context.Addresses.AddAsync(newAddress);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{AddressId}")]
        public async Task<ActionResult<Address>> DeleteAddress(int addressId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            userId = "ce693acd-d08d-4303-9e5d-a26b832abc38";
            var foundAddress = await _context.Addresses.FirstOrDefaultAsync(x => x.AddressId == addressId && x.UserId == userId);

            if (foundAddress == null)
            {
                return NotFound();
            }

            _context.Remove(foundAddress);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}