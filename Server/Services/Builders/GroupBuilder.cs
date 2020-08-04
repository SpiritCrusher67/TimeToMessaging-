using Microsoft.AspNetCore.Http;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTMLibrary.ModelViews;

namespace Server.Services.Builders
{
    public class GroupBuilder : IEntityBuilder<Group,GroupModelView>
    {
        protected ApplicationContext _context;

        public void ConfigureBuilder(ApplicationContext context, Microsoft.AspNetCore.Hosting.IWebHostEnvironment environment = null)
        {
            _context = context;
        }

        public Task<Group> Create(GroupModelView modelView)
        {
            var group = new Group
            {
                Id = Guid.NewGuid(),
                CreatorId = modelView.CreatorLogin,
                Name = modelView.Name
            };

            return Task.FromResult(group);
            
        }

        public async Task<Group> GetEntity(object id) => await _context.Groups.FindAsync(id);

        public async Task<GroupModelView> GetModelView(object id)
        {
            var group = await GetEntity(id);

            if (group == null)
                return null;

            return new GroupModelView
            {
                Id = group.Id,
                CreatorLogin = group.CreatorId,
                Name = group.Name
            }; 
        }
    }
}
