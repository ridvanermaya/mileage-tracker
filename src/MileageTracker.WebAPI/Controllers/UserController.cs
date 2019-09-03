using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace MileageTracker.WebAPI.Controllers
{
    public class UserController : MTBaseController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public UserController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserModel user)
        {
            var identityEmail = User.FindFirstValue(ClaimTypes.Email);

            if (user.Email == identityEmail)
            {
                return Ok(new
                {
                    success = false,
                    message = "Email already exists!"
                });
            }

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

            var userData = await _userManager.FindByNameAsync(user.Email);
            var token = GenerateToken(userData);

            return Ok(new
            {
                Id = userData.Id,
                Username = userData.UserName,
                Email = userData.Email,
                Token = token,
                Success = true
            });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]UserModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);

            if (!result.Succeeded)
                return BadRequest(new { message = "Username or password is incorrect" });

            var user = await _userManager.FindByNameAsync(model.Email);
            var token = GenerateToken(user);

            return Ok(new
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Token = token,
                Success = true
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return Ok(new
            {
                Success = true
            });
        }

        public string GenerateToken(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Startup.AuthenticationSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            
            return tokenString;
        }
    }

    public class UserModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}