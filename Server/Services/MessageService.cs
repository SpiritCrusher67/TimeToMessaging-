using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Server.Models;
using Server.Services.Builders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using TTMLibrary.ModelViews;

namespace Server.Services
{
    public class MessageService : IEntityService<Message,MessageModelView>, IEntityFilesHandler
    {
        ApplicationContext _context;
        FileService _fileService;
        public MessageService(ApplicationContext context, FileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task<(Stream,string)> GetFile(string id)
        {
            var message = await _context.Messages.FindAsync(id);

            if (message == null)
                return (null, null);

            return  await _fileService.GetAttachedFile(message.Id.ToString(), message.AttachedFileName);
        }

        public async Task SaveFile(IFormFile file, string innerDirectory, string newFileName = null)
        {
            await _fileService.SaveAttachedFile(file.OpenReadStream(),innerDirectory, file.FileName);
        }

        public async Task<MessageModelView> Get(object id, IEntityBuilder<Message, MessageModelView> builder)
        {
            builder.ConfigureBuilder(_context);

            return await builder.GetModelView(id);
        }

        public async Task<MessageModelView> Create(MessageModelView modelView, IEntityBuilder<Message, MessageModelView> builder)
        {
            builder.ConfigureBuilder(_context);

            var message = await builder.Create(modelView);

            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();

            return await Get(message.Id, builder);
        }

        public Task<MessageModelView> Update(MessageModelView modelView)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Delete(object id, string userLogin)
        {
            var message = await _context.Messages.FindAsync(id);

            if (message == null || message.UserId != userLogin)
                return false;

            await _fileService.DeleteAttachedFile(message.Id.ToString());

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return true;
        }


    }
}
