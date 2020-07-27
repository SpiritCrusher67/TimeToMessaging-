using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTMLibrary.Models;

namespace Server.Services.Builders
{
    public class FullUserBuilder : UserBuilder
    {
        public override async Task<User> GetUser(string login)
        {
            var userModel = await _context.Users
                .Include("Friends")
                .Include("Groups")
                .Include("Invites")
                .Include("SendedInvites")
                .Where(u => u.Login == login)
                .SingleOrDefaultAsync();
            if (userModel == null) return null;

            var user = (User)userModel;
            await AppendFriends(userModel, user);

            return user;
        }

        private async Task AppendFriends(UserModel userModel, User user)
        {
            foreach (var friend in userModel.Users1)
            {
                user.Friends.Add(await base.GetUser(friend.Friend.Login));
            }
        }
    }
}
