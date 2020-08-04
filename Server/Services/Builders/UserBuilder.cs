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
using Microsoft.AspNetCore.Http;

namespace Server.Services.Builders
{
    public class UserBuilder : IEntityBuilder<User,UserModelView>
    {
        protected ApplicationContext _context;
        protected IWebHostEnvironment _environment;
        public void ConfigureBuilder(ApplicationContext context, IWebHostEnvironment environment = null)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<User> Create(UserModelView modelView)
        {
            var regModelView = modelView as RegistrationModelView;

            if (regModelView != null && await _context.Users.Where(u => u.Login == regModelView.Login).FirstOrDefaultAsync() != null)
                return null;

            return new User
            {
                Login = regModelView.Login,
                Email = regModelView.Email,
                IsConfirmed = false,
                Password = regModelView.Password
            };
        }

        public async Task<User> GetEntity(object id)
        {
            return await _context.Users.FindAsync(id);
        }


        public virtual async Task<UserModelView> GetModelView(object id)
        {
            var user = await _context.Users.FindAsync(id);
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
