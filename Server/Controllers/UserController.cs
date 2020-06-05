using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Models;
using TTMLibrary.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        ApplicationContext db;
        public UserController(ApplicationContext context)
        {
            db = context;
        }

        [HttpGet("{login}")]
        public async Task<ActionResult<IEnumerable<string>>> Get(string login)
        {
            var user = await db.Users.Include("Friends").Where(u => u.Login == login).FirstOrDefaultAsync();
            if (user.Friends?.Count > 0)
            {
                return user.Friends.Select(f => f.FriendId).ToList();

            }
            return null;
        }

        [HttpPost]
        public async Task Post(User user)
        {
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }

        [HttpGet("{login}/{friendId}")]
        public async Task<ActionResult<User>> Get(string login, string friendId)
        {
            return await db.Users.FindAsync(friendId);
        }

        [HttpGet("Search/{text}")]
        public async Task<ActionResult<ICollection<User>>> Search(string text)
        {
            return await db.Users.Where(u => u.Login.Contains(text)).ToListAsync();
        }

        [HttpGet("AddFriend/{login}/{friendId}")]
        public async Task Add(string login, string friendId)
        {
            var user = await db.Users.Include("Friends").Where(u => u.Login == login).FirstOrDefaultAsync();
            var friend = await db.Users.Include("Friends").Where(u => u.Login == friendId).FirstOrDefaultAsync();
            user.Friends.Add(new UserUser { UserId = login, FriendId = friendId });
            friend.Friends.Add(new UserUser { UserId = friendId, FriendId = login });
            db.Update(user);
            db.Update(friend);

            await db.SaveChangesAsync();
        }
    }
}