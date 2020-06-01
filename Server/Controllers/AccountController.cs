using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TTMLibrary.Models;

namespace Server.Controllers
{
    [Controller]
    public class AccountController : Controller
    {
        private ApplicationContext _context;
        public AccountController(ApplicationContext context)
        {
            _context = context;
        }
        [HttpPost("/token")]
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
                    audience: AuthOptions.AUDIENCE,
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
            User person = await _context.Users.FirstOrDefaultAsync(x => x.Login == login && x.Password == password);
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
    }
}
