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
    public class MessageBuilder : IEntityBuilder<Message,MessageModelView>
    {
        protected ApplicationContext _context;

        public void ConfigureBuilder(ApplicationContext context,IWebHostEnvironment environment = null)
        {
            _context = context;
        }

        public Task<Message> Create(MessageModelView modelView)
        {
            return Task.FromResult(new Message
            {
                Id = Guid.NewGuid(),
                GroupId = modelView.GroupId,
                UserId = modelView.SenderLogin,
                Date = DateTime.UtcNow,
                Text = modelView.Text,
                AttachedFileName = modelView.AttachedFileName
            });
            
        }

        public async Task<Message> GetEntity(object id) => await _context.Messages.FindAsync(id);

        public async Task<MessageModelView> GetModelView(object id)
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
