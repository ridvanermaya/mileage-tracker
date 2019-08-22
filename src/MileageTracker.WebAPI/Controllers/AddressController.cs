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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var address = await _context.Addresses.Where(x => x.UserId == userId).FirstOrDefaultAsync();

            if (address == null)
            {
                return NotFound();
            }

            return address;
        }

        [HttpPut("{AddressId}")]
        public async Task<ActionResult<Address>> EditAddress(int addressId, Address address)
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
        public async Task<ActionResult<Address>> PostAddress(Address address)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
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
            var foundAddress = await _context.Addresses.FindAsync(addressId);

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