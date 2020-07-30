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
    public class MessageService
    {
        ApplicationContext _context;
        IWebHostEnvironment _environment;
        public MessageService(ApplicationContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<MessageModelView> GetMessage(Guid messageId, IMessageBuilder builder)
        {
            builder.ConfigureBuilder(_context, _environment);

            return await builder.GetMessage(messageId);
        }

        public async Task<MessageModelView> CreateMessage(MessageModelView modelView, IMessageBuilder builder, IFormFile attachedFile = null)
        {
            builder.ConfigureBuilder(_context, _environment);

            var message = await builder.CreateMessage(modelView,attachedFile);

            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();

            return await GetMessage(message.Id, builder);
        }

        public async Task<(Stream,string)> GetAttachedFile(Guid messageId)
        {
            var msg = await _context.Messages.FindAsync(messageId);

            if (msg == null)
                return (null,null);

            new FileExtensionContentTypeProvider().TryGetContentType(msg.AttachedFileName,out string contentType);

            return (File.OpenRead($"{_environment.WebRootPath}/Files/AttachedFiles/{msg.Id}/{msg.AttachedFileName}"), contentType);
        }

        public async Task<bool> DeleteMessage(Guid messageId, string userLogin)
        {
            var message = await _context.Messages.FindAsync(messageId);

            if (message == null || message.UserId != userLogin)
                return false;

            var filesDirectory = $"{_environment.WebRootPath}/Files/AttachedFiles/{message.Id}";
            if (Directory.Exists(filesDirectory))
                Directory.Delete(filesDirectory, true);

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}
