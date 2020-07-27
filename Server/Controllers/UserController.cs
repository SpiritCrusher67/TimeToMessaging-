using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Services;
using Server.Services.Builders;
using Server.Models;
using TTMLibrary.Models;
using Microsoft.AspNetCore.Hosting;

namespace Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        UsersService _usersService;
        public UserController(UsersService usersService)
        {
            _usersService = usersService;
        }

        [AllowAnonymous]
        [HttpPost("/[controller]/token")]
        public async Task<IActionResult> Token(string login, string password)
        {
            var identity = await GetIdentity(login, password);
            if (identity == null)
            {
                return BadRequest("Invalid username or password.");
            }

            var now = DateTime.UtcNow;
            // создаем токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            //var response = new
            //{
            //    access_token = encodedJwt,
            //    login = identity.Name
            //};
            return Ok(encodedJwt);
        }

        private async Task<ClaimsIdentity> GetIdentity(string login, string password)
        {
            User person = await _usersService.GetUser(login, password);
            if (person != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, person.Login),
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token");
                return claimsIdentity;
            }
            // если пользователь не найден
            return null;
        }

        [HttpGet]
        public async Task<IActionResult> GetFullUserData()
        {
            return Ok(await _usersService.GetUser(HttpContext.User.Identity.Name, new FullUserBuilder()));
        }

        [HttpGet("/[controller]/getUser/{login}")]
        public async Task<IActionResult> GetUserData(string login)
        {
            return Ok(await _usersService.GetUser(login, new UserBuilder()));
        }

    }
}