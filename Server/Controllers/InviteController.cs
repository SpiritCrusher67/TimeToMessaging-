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
    public class InviteController : ControllerBase
    {
        ApplicationContext db;
        public InviteController(ApplicationContext context)
        {
            db = context;
        }

        //создание инвайта
        [HttpPost]
        public async Task<ActionResult<Invite>> Post([FromBody]JObject data)
        {
            var groupId = data["groupId"].ToObject<Guid>();
            var userLogin = data["userLogin"].ToString();

            var invite = new Invite(userLogin,groupId);

            db.Invites.Add(invite);
            await db.SaveChangesAsync();

            return Ok();
        }

        //обновление
        [HttpPut]
        public async Task<ActionResult<Invite>> Put([FromBody]JObject data)
        {
            var invite = data["invite"].ToObject<Invite>();

            db.Update(invite);
            await db.SaveChangesAsync();

            return Ok();
        }
    }
}