using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Server.Models;
using TTMLibrary.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        ApplicationContext db;
        public MessageController(ApplicationContext context)
        {
            db = context;
        }

        //создание инвайта
        [HttpPost]
        public async Task<ActionResult<Message>> Post([FromBody]JObject data)
        {
            var groupId = data["groupId"].ToObject<Guid>();
            var userLogin = data["userLogin"].ToString();
            var text = data["text"].ToString();

            var message = new Message(userLogin, groupId);
            message.Date = DateTime.Now;
            message.Text = text;

            db.Messages.Add(message);
            await db.SaveChangesAsync();

            return Ok();
        }

    }
}