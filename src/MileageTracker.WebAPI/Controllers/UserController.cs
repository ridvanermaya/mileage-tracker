using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MileageTracker.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        public UserController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post(UserModel user)
        {
            var identityUser = new IdentityUser()
            {
                UserName = user.Email,
                Email = user.Email,
            };
            var result = await _userManager.CreateAsync(identityUser, user.Password);

            if (!result.Succeeded)
            {
                return Ok(new
                {
                    success = false,
                    message = "There is a problem with your information."
                });
            }

            return Ok(new
            {
                success = true,
                message = "User created sucesfully."
            });
        }
    }

    public class UserModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}