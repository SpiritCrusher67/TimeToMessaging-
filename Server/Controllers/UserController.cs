using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Server.Services;
using Server.Services.Builders;
using Server.Models;
using TTMLibrary.ModelViews;
using System.IO;

namespace Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/[controller]")]
    public class UserController : ControllerBase
    {
        IEntityService<User, UserModelView> _usersService;
        public UserController(IEntityService<User,UserModelView> usersService)
        {
            _usersService = usersService;
        }

        [AllowAnonymous]
        [HttpPost("/api/[controller]/token")]
        public async Task<IActionResult> Token(string login, string password)
        {
            var identity = await GetIdentity(login, password);
            if (identity == null)
            {
                return BadRequest("Invalid username or password.");
            }

            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return Ok(encodedJwt);
        }
        private async Task<ClaimsIdentity> GetIdentity(string login, string password)
        {
            User person = await ((UsersService)_usersService).GetUser(login, password);
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

            return null;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateUser(RegistrationModelView modelView)
        {
            if (ModelState.IsValid && await _usersService.Create(modelView, new UserBuilder()) != null)
                return Ok();

            return ValidationProblem();
        }

        [HttpGet]
        public async Task<IActionResult> GetFullUserData()
        {
            var modelView = await _usersService.Get(HttpContext.User.Identity.Name, new FullUserBuilder());

            if (modelView == null)
                return BadRequest();
            return Ok(modelView);
        }

        [HttpGet("/api/[controller]/getUser/{login}")]
        public async Task<IActionResult> GetUserData(string login)
        {
            var modelView = await _usersService.Get(login, new UserBuilder());

            if (modelView == null)
                return NotFound();
            return Ok(modelView);
        }

        [HttpPost("/api/[controller]/uploadAvatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile avatar)
        {
            if (avatar == null)
                return BadRequest();
            if (!".jpg|.png|.jpeg".Contains(Path.GetExtension(avatar.FileName).ToLower()))
                return BadRequest("Не подходящее расширение файла");

            await ((IEntityFilesHandler)_usersService).SaveFile(avatar, HttpContext.User.Identity.Name);

            return Ok();
        }

        [HttpPut("/api/[controller]/changePassword")]
        public async Task<IActionResult> ChangePassword(PasswordModelView modelView)
        {
            if (ModelState.IsValid && modelView.Login == HttpContext.User.Identity.Name && await _usersService.Update(modelView) != null)
                return Ok();

            return BadRequest();
        }

        [HttpPut("/api/[controller]/changeEmail")]
        public async Task<IActionResult> ChangeEmail(EmailModelView modelView)
        {
            if (ModelState.IsValid && await _usersService.Update(modelView) != null)
                return Ok();

            return BadRequest();
        }
    }
}