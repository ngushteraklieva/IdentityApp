using API.DTOs.Account;
using API.Extensions;
using API.Models;
using API.Services.IServices;
using API.Utility;
using Azure.Messaging;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("auth-status")]
        public IActionResult isLoggedIn()
        {
            return Ok(new { isAuthenticated = User.Identity?.IsAuthenticated ?? false });
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
                return Unauthorized(new DTOs.ApiResponse(401, message: "Invalid username or password"));
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (!result.Succeeded)
            {
                RemoveJWTCookie();
                return Unauthorized(new DTOs.ApiResponse(401, message:"Invalid username or password"));

            }

            return CreateAppUserDto(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (await CheckEmailExistsAsync(model.Email))
            {
                return BadRequest(new DTOs.ApiResponse(400, message: $"An account has been registered with '{model.Email}'."));
            }

            if (await CheckNameExistsAsync(model.Name))
            {
                return BadRequest(new DTOs.ApiResponse(400, message:$"An account has been registered with '{model.Name}'."));
            }

            var userToAdd = new AppUser
            {
                Name = model.Name,
                UserName = model.Name.ToLower(),
                Email = model.Email,
                EmailConfirmed = true,
                LockoutEnabled = true
            };

            var result = await _userManager.CreateAsync(userToAdd, model.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(new DTOs.ApiResponse(201, message:"Your account has been created"));
        }

        //new -> Create a small object right now, without defining a class for it
        [HttpGet("name-taken")]
        public async Task<IActionResult> NameTaken([FromQuery] string name)
        {
            return Ok(new { IsTaken = await CheckNameExistsAsync(name) });
        }

        [HttpGet("email-taken")]
        public async Task<IActionResult> EmailTaken([FromQuery] string email)
        {
            return Ok(new { IsTaken = await CheckEmailExistsAsync(email) });
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            RemoveJWTCookie();
            return NoContent();
        }


        //This endpoint revalidates the authenticated user against the database
        //and issues a fresh JWT cookie to keep the session alive.
        //The server creates a brand-new JWT with a new expiration time and overwrites the old cookie.
        [Authorize]
        [HttpGet("refresh-appuser")]
        public async Task<ActionResult> RefreshAppUSer()
        {
            var user = await _userManager.Users.Where(x => x.Id == User.GetUserId())
                .FirstOrDefaultAsync();

            if(user == null)
            {
                RemoveJWTCookie();
                return Unauthorized(new ApiResponse(401));
            }

            return Ok(CreateAppUserDto(user));

        }

        #region Private Methods
        private AppUserDto CreateAppUserDto(AppUser user)
        {
            string jwt = _tokenService.CreateJWT(user);
            SetJWTCookie(jwt);

            return new AppUserDto
            {
                Name = user.Name,
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

        private async Task<bool> CheckNameExistsAsync(string name)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == name.ToLower());
        }
        #endregion
    }
}
