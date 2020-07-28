using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Server.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TTMLibrary.ModelViews;
using System.Threading.Tasks;

namespace Server.Services.Builders
{
    public class UserBuilder : IUserBuilder
    {
        protected ApplicationContext _context;
        protected IWebHostEnvironment _environment;
        public UserBuilder()
        {

        }

        public void ConfigureBuilder(ApplicationContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<User> CreateUser(RegistrationModelView modelView)
        {
            if (await _context.Users.Where(u => u.Login == modelView.Login).FirstOrDefaultAsync() != null)
                return null;

            return new User
            {
                Login = modelView.Login,
                Email = modelView.Email,
                IsConfirmed = false,
                Password = modelView.Password
            };
        }

        public virtual async Task<UserModelView> GetUser(string login)
        {
            var user = await _context.Users.Where(u => u.Login == login).SingleOrDefaultAsync();
            if (user == null) return null;
            var modelView = new UserModelView(user.Login);

            await AppendAvatar(modelView);

            return modelView;
        }

        protected async Task AppendAvatar(UserModelView user)
        {
            var avatartPatch = Directory.GetFiles(_environment.WebRootPath + "/Files/Avatars/", $"{user.Login}.*").FirstOrDefault() ??
                _environment.WebRootPath + "/Files/DefaultFiles/DefaultAvatar.png";

            using (var fileStream = File.OpenRead(avatartPatch))
            {
                user.Avatar = new byte[fileStream.Length];
                await fileStream.ReadAsync(user.Avatar, 0, user.Avatar.Length);
            }
        }

    }
}
