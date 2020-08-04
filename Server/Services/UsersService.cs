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
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Server.Services
{
    public class UsersService : IEntityService<User,UserModelView>, IEntityFilesHandler
    {
        ApplicationContext _context;
        IWebHostEnvironment _environment;

        public UsersService(ApplicationContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;

        }

        public async Task<User> GetUser(string login,string password) => 
            await _context.Users.Where(u => u.Login == login && u.Password == password).SingleOrDefaultAsync();


        public async Task<UserModelView> Get(object Id, IEntityBuilder<User, UserModelView> builder)
        {
            builder.ConfigureBuilder(_context, _environment);

            return await builder.GetModelView(Id);
        }

        public async Task<UserModelView> Create(UserModelView modelView, IEntityBuilder<User, UserModelView> builder)
        {
            builder.ConfigureBuilder(_context);

            var user = await builder.Create(modelView);
            if (user == null)
                return null;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return modelView;
        }

        public async Task<UserModelView> Update(UserModelView modelView)
        {
            var user = await _context.Users.FindAsync(modelView.Login);
            if (user == null)
                return null;

            if (modelView is PasswordModelView)
            {
                var passMV = (PasswordModelView)modelView;

                if (user.Password != passMV.OldPassword)
                    return null;

                user.Password = passMV.Password;
            }
            else if (modelView is EmailModelView)
            {
                var emailMV = (EmailModelView)modelView;

                user.Email = emailMV.NewEmail;
            }

            await _context.SaveChangesAsync();

            return modelView;
        }

        public Task<bool> Delete(object id, string userLogin)
        {
            throw new NotImplementedException();
        }

        public Task<(Stream, string)> GetFile(string id)
        {
            throw new NotImplementedException();
        }

        public async Task SaveFile(IFormFile file, string innerDirectory = null, string newFileName = null)
        {
            var path = $"{_environment.WebRootPath}/Files/Avatars/";
            var avatarPath = Directory.GetFiles(path, $"{newFileName}.*").FirstOrDefault();

            if (avatarPath != string.Empty)
                File.Delete(avatarPath);

            using (FileStream fs = new FileStream($"{path}{newFileName}{Path.GetExtension(file.FileName)}", FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }
        }
    }
}
