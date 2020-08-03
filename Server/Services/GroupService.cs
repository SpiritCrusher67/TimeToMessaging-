using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Server.Models;
using Server.Services.Builders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TTMLibrary.ModelViews;

namespace Server.Services
{
    public class GroupService : IEntityService<Group,GroupModelView>, IEntityFilesHandler
    {
        ApplicationContext _context;
        FileService _fileService;
        public GroupService(ApplicationContext context, FileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task<GroupModelView> Create(GroupModelView modelView, IEntityBuilder<Group, GroupModelView> builder)
        {
            builder.ConfigureBuilder(_context);

            var group = await builder.Create(modelView);

            await _context.Groups.AddAsync(group);
            await _context.SaveChangesAsync();

            modelView.Id = group.Id;

            return modelView;
        }

        public async Task<bool> Delete(object id, string userLogin)
        {
            var group = await _context.Groups.FindAsync(id);

            if (group == null || group.CreatorId != userLogin)
                return false;

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();

            await _fileService.DeleteGroupAvatar(group.Id.ToString());

            return true;
        }

        public async Task<GroupModelView> Get(object Id, IEntityBuilder<Group, GroupModelView> builder)
        {
            builder.ConfigureBuilder(_context);

            return await builder.GetModelView(Id);
        }

        public Task<(Stream, string)> GetFile(string id)
        {
            return _fileService.GetGroupAvatar(id);
        }

        public async Task SaveFile(IFormFile file, string innerDirectory = null,string newFileName = null)
        {
            var fileName = (newFileName != string.Empty) ? newFileName + Path.GetExtension(file.FileName) : file.FileName;

            await _fileService.SaveGroupAvatar(file.OpenReadStream(), fileName);
        }

        public async Task<GroupModelView> Update(GroupModelView modelView)
        {
            var group = await _context.Groups.FindAsync(modelView.Id);

            if (group == null)
                return null;

            group.Name = modelView.Name;

            await _context.SaveChangesAsync();

            return modelView;
        }
    }
}
