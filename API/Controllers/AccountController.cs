using API.DTOs.Account;
using API.Models;
using API.Services.IServices;
using API.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        //Dependency injection
        //Inside this controller, I want to store a tool called UserManager<AppUser>
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _config;

        //Constructor Injection
        //When this controller is created, ASP.NET: please give me a UserManager
        //the standard way ASP.NET gives services to your controllers
        //UserManager<AppUser> - A built-in Identity service for managing users
        public AccountController(UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager,
            ITokenService tokenService,
            IConfiguration iconfig)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _config = iconfig;
        }


        [HttpPost("login")]
        public async Task<ActionResult<AppUserDto>> Login(LoginDto model)
        {
            var user = await _userManager.Users
                .Where(x => x.UserName == model.UserName)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                user = await _userManager.Users
                .Where(x => x.UserName == model.UserName)
                .FirstOrDefaultAsync();
            }

            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (!result.Succeeded)
            {
                RemoveJWTCookie();
                return Unauthorized("Invalid username or password");

            }

            return CreateAppUserDto(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (await CheckEmailExistsAsync(model.Email))
            {
                return BadRequest($"An account has been registered with '{model.Email}'.");
            }

            if (await CheckUsernameExistsAsync(model.UserName))
            {
                return BadRequest($"An account has been registered with '{model.UserName}'.");
            }

            var userToAdd = new AppUser
            {
                UserName = model.UserName,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(userToAdd, model.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok("Your account has been created");
        }

        #region Private Methods
        private AppUserDto CreateAppUserDto(AppUser user)
        {
            string jwt = _tokenService.CreateJWT(user);
            SetJWTCookie(jwt);

            return new AppUserDto
            {
                UserName = user.UserName,
                JWT = jwt
            }; 
        }
        private void SetJWTCookie(string jwt)
        {
            var cookieOptions = new CookieOptions
            {
                IsEssential = true,
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddDays(int.Parse(_config["JWT:ExpiresInDays"]))
            };
            Response.Cookies.Append(SD.IdentityAppCookie, jwt, cookieOptions);

        }

        private void RemoveJWTCookie()
        {
            Response.Cookies.Delete(SD.IdentityAppCookie);
        }

        //Here _userManager comes from the constructor.
        private async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await _userManager.Users.AnyAsync(x => x.Email == email);
        }

        private async Task<bool> CheckUsernameExistsAsync(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == username);
        }
        #endregion
    }
}
