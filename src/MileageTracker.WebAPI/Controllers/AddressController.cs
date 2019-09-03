using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MileageTracker.WebAPI.Models;

namespace MileageTracker.WebAPI.Controllers
{
    public class AddressController : MTBaseController
    {
        public readonly ApplicationDbContext _context;

        public AddressController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Address>> GetAddress()
        {
            var userId = User.FindFirstValue(ClaimTypes.Name); 
            var address = await _context.Addresses.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == userId);

            if (userId == null)
            {
                return Unauthorized();
            }

            return address;
        }

        [HttpGet("{AddressId}")]
        public async Task<ActionResult<Address>> GetAddressById(int addressId)
        {
            var userId = User.FindFirstValue(ClaimTypes.Name); 
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

            var userId = User.FindFirstValue(ClaimTypes.Name); 

            _context.Entry(address).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Address>> AddAddress(Address address)
        {
            var userId = User.FindFirstValue(ClaimTypes.Name);    
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
            newAddress.UserId = userId.ToString();

            await _context.Addresses.AddAsync(newAddress);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{AddressId}")]
        public async Task<ActionResult<Address>> DeleteAddress(int addressId)
        {
            var userId = User.FindFirstValue(ClaimTypes.Name); 
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