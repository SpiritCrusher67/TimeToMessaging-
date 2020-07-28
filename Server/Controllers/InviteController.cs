using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Server.Models;

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
        public async Task<ActionResult<Invite>> Post(Invite invite)
        {
            db.Invites.Add(invite);
            await db.SaveChangesAsync();
            return Ok();
        }
        [HttpPut]
        public async Task<ActionResult<Invite>> Put(Invite invite)
        {
            db.Update(invite);
            await db.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("{login}")]
        public async Task<ActionResult<IEnumerable<Guid>>> Get(string login)
        {
            var user = await db.Users.Include("Invites").Where(u => u.Login == login).FirstOrDefaultAsync();
            return user.Invites.Where(i=> i.Status == IviteStatus.Sended).Select(g => g.Id).ToList();
        }
        [HttpGet("{login}/{id}")]
        public async Task<ActionResult<Invite>> Get(string login, Guid id)
        {
            var invite = await db.Invites.Include("Group").Where(g => g.Id == id).FirstOrDefaultAsync();
            return invite;
        }
    }
}