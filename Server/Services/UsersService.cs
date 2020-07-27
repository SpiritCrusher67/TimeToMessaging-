using Server.Services.Builders;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTMLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

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

        public async Task<User> GetUser(string login, IUserBuilder userBuilder)
        {
            userBuilder.ConfigureBuilder(_context, _environment);

            return await userBuilder.GetUser(login);
        }

        public async Task<User> GetUser(string login,string password) => 
            await _context.Users.Where(u => u.Login == login && u.Password == password).SingleOrDefaultAsync();

        public User CreateUser()
        {
            throw new NotImplementedException();

        }

        public async Task<User> AddFriend(string login, string friendLogin)
        {
            throw new NotImplementedException();
        }

    }
}
