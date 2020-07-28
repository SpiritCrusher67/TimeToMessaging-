using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTMLibrary.ModelViews;

namespace Server.Services.Builders
{
    public class FullUserBuilder : UserBuilder
    {
        public override async Task<UserModelView> GetUser(string login)
        {
            var user = await _context.Users
                .Include("Users1")
                .Include("Users2")
                .Include("Groups")
                .Include("Invites")
                .Include("SendedInvites")
                .Where(u => u.Login == login)
                .SingleOrDefaultAsync();
            if (user == null) return null;
            var modelView = new UserModelView(user.Login);

            await AppendAvatar(modelView);
            await AppendFriends(modelView,user);

            return modelView;
        }

        private async Task AppendFriends(UserModelView modelView, User user)
        {
            if (user?.Users1?.Count > 0)
                foreach (var friend in user.Users1)
                {
                    modelView.Friends.Add(await base.GetUser(friend.FriendId));
                }
        }
    }
}
