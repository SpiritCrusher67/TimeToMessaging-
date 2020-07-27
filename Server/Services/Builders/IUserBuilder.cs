using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTMLibrary.Models;

namespace Server.Services.Builders
{
    public interface IUserBuilder
    {
        public void ConfigureBuilder(ApplicationContext context,IWebHostEnvironment environment);

        public Task<User> GetUser(string login);

    }
}
