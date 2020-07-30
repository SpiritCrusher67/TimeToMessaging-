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
    public interface IMessageBuilder
    {
        public void ConfigureBuilder(ApplicationContext context, IWebHostEnvironment environment);
        public Task<MessageModelView> GetMessage(Guid id);
        public Task<Message> CreateMessage(MessageModelView modelView, IFormFile file = null);
        
    }
}
