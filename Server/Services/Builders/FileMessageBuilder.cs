using Microsoft.AspNetCore.Http;
using Server.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TTMLibrary.ModelViews;

namespace Server.Services.Builders
{
    public class FileMessageBuilder : TextMessageBuilder
    {
        public override async Task<Message> CreateMessage(MessageModelView modelView, IFormFile file)
        {
            var message = await base.CreateMessage(modelView);
            message.AttachedFileName = file.FileName;

            var directory = Directory.CreateDirectory($"{_environment.WebRootPath}/Files/AttachedFiles/{message.Id}");

            using (FileStream fs = new FileStream($"{directory.FullName}/{file.FileName}", FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }
            
            return message;
        }
    }
}
