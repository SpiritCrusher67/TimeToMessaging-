using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Server.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TTMLibrary.Models;

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

        public virtual async Task<User> GetUser(string login)
        {
            var user = (User)await _context.Users.Where(u => u.Login == login).SingleOrDefaultAsync();
            if (user == null) return null;

            AppendAvatar(user);

            return user;
        }

        protected async void AppendAvatar(User user)
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
