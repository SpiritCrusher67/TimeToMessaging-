using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        ApplicationContext db;
        public GroupController(ApplicationContext context)
        {
            db = context;
        }

        //создание группы
        [HttpPost]
        public async Task<ActionResult<Group>> Post(Group group)
        {
            db.Groups.Add(group);
            await db.SaveChangesAsync();

            group.Users.Add(new UserGroup { GroupId = group.Id, UserLogin = group.CreatorId });
            await db.SaveChangesAsync();

            return Ok(group);
        }

        [HttpGet("{login}")]
        public async Task<ActionResult<IEnumerable<Guid>>> Get(string login)
        {
            var user = await db.Users.Include("Groups").Where(u => u.Login==login).FirstOrDefaultAsync();
            return user?.Groups.Select(g => g.GroupId).ToList();
        }
        [HttpGet("{login}/{id}")]
        public async Task<ActionResult<Group>> Get(string login, Guid id)
        {
            var group = await db.Groups.Include("Users").Where(g => g.Id == id).FirstOrDefaultAsync();
            return group;
        }
        [HttpGet("UsersList/{groupId}")]
        public async Task<ActionResult<IEnumerable<string>>> Get(Guid groupId)
        {
            var group = await db.Groups.Include("Users").Where(g => g.Id == groupId).FirstOrDefaultAsync();
            return group.Users.Select(g => g.UserLogin).ToList();
        }
        [HttpGet("AddUser/{groupId}/{userLogin}")]
        public async Task<Group> Get(Guid groupId, string userLogin)
        {
            var group = await db.Groups.Include("Users").Where(g => g.Id == groupId).FirstOrDefaultAsync();
            group.Users.Add(new UserGroup(groupId, userLogin));
            db.Update(group);
            await db.SaveChangesAsync();
            return group;
        }
        //обновление группы
        [HttpPut]
        public async Task<ActionResult<Group>> Put([FromBody]JObject data)
        {
            var group = data["group"].ToObject<Group>();
            var userLogin = data["userLogin"].ToString();

            if (group == null) return BadRequest();

            if (!await db.Groups.AnyAsync(g => g.Id == group.Id && g.CreatorId == userLogin)) return BadRequest(); // существует ли группа и создал ли ее пользователь отправивший запрос

            db.Update(group);

            await db.SaveChangesAsync();

            return Ok(group);
        }


    }
}