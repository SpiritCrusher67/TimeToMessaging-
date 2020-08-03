using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Server.Models;
using System;
using System.Threading.Tasks;
using TTMLibrary.ModelViews;

namespace Server.Services.Builders
{
    public interface IGroupBuilder
    {
        public void ConfigureBuilder(ApplicationContext context, IWebHostEnvironment environment);
        public Task<GroupModelView> GetGroup(Guid id);
        public Task<Group> CreateGroup(GroupModelView modelView, IFormFile groupAvatar = null);
    }
}
