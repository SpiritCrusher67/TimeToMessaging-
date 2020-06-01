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
            return user.Friends.Select(f => f.FriendId).ToList();
        }

        [HttpGet("{login}/{friendId}")]
        public async Task<ActionResult<User>> Get(string login, string friendId)
        {
            return await db.Users.FindAsync(friendId);
        }
    }
}