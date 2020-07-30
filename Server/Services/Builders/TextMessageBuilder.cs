using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTMLibrary.ModelViews;

namespace Server.Services.Builders
{
    public class TextMessageBuilder : IMessageBuilder
    {
        protected ApplicationContext _context;
        protected IWebHostEnvironment _environment;

        public void ConfigureBuilder(ApplicationContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public virtual Task<Message> CreateMessage(MessageModelView modelView, IFormFile file = null)
        {
            var message = new Message
            {
                Id = Guid.NewGuid(),
                GroupId = modelView.GroupId,
                UserId = modelView.SenderLogin,
                Date = DateTime.UtcNow,
                Text = modelView.Text,
                AttachedFileName = modelView.AttachedFileName
            };

            return Task.FromResult(message);
        }

        public virtual async Task<MessageModelView> GetMessage(Guid id)
        {
            var message = await _context.Messages.FindAsync(id);

            if (message == null)
                return null;

            return new MessageModelView 
            { 
                Id = message.Id,
                SenderLogin = message.UserId,
                Date = message.Date,
                GroupId = message.GroupId,
                Text = message.Text,
                AttachedFileName = message.AttachedFileName
            };
        }


    }
}
