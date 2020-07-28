using Server.Services.Builders;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using TTMLibrary.ModelViews;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Server.Services
{
    public class UsersService
    {
        ApplicationContext _context;
        IWebHostEnvironment _environment;

        public UsersService(ApplicationContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;

        }

        public async Task<UserModelView> GetUser(string login, IUserBuilder userBuilder)
        {
            userBuilder.ConfigureBuilder(_context, _environment);

            return await userBuilder.GetUser(login);
        }

        public async Task<User> GetUser(string login,string password) => 
            await _context.Users.Where(u => u.Login == login && u.Password == password).SingleOrDefaultAsync();

        public async Task<bool> CreateUser(RegistrationModelView modelView, IUserBuilder userBuilder)
        {
            userBuilder.ConfigureBuilder(_context, _environment);

            var user = await userBuilder.CreateUser(modelView);

            if (user != null)
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return true;
            }
            return false;
        }

        public async Task<User> AddFriend(string login, string friendLogin)
        {
            throw new NotImplementedException();
        }

        public async Task UploadAvatar(IFormFile avatar, string login)
        {
            var path = $"{_environment.WebRootPath}/Files/Avatars/";
            var file = Directory.GetFiles(path , $"{login}.*").FirstOrDefault();

            if (file != string.Empty)
                File.Delete(file);

            using (FileStream fs = new FileStream($"{path}{login}{Path.GetExtension(avatar.FileName)}", FileMode.Create))
            {
                await avatar.CopyToAsync(fs);
            }
        }

        public async Task<bool> ChangePassword(PasswordModelView modelView, string login)
        {
            var user = await _context.Users.FindAsync(login);

            if (user == null || user.Password != modelView.OldPassword)
                return false;

            user.Password = modelView.Password;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ChangeEmail(EmailModelView modelView, string login)
        {
            var user = await _context.Users.FindAsync(login);

            if (user == null || user.Password != modelView.Password)
                return false;

            user.Email = modelView.NewEmail;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
