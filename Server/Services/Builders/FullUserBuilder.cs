using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TTMLibrary.ModelViews;

namespace Server.Services.Builders
{
    public class FullUserBuilder : UserBuilder
    {
        public override async Task<UserModelView> GetModelView(object id)
        {
            var user = await _context.Users
                .Include("Users1")
                .Include("Users2")
                .Include("Groups")
                .Include("Invites")
                .Include("SendedInvites")
                .Where(u => u.Login == (string)id)
                .SingleOrDefaultAsync();
            if (user == null) return null;
            var modelView = new UserModelView(user.Login);

            await AppendAvatar(modelView);
            await AppendFriends(modelView,user);
            await AppendGroups(modelView, user);
            await AppendInvites(modelView, user);

            return modelView;
        }

        private async Task AppendFriends(UserModelView modelView, User user)
        {
            if (user?.Users1?.Count > 0)
                foreach (var friend in user.Users1)
                {
                    modelView.Friends.Add(await base.GetModelView(friend.FriendId));
                }
        }

        private async Task AppendGroups(UserModelView modelView, User user) 
        {
            var groupBuilder = new GroupBuilder();
            groupBuilder.ConfigureBuilder(_context);

            if (user?.Groups?.Count > 0)
                foreach (var group in user.Groups)
                {
                    modelView.Groups.Add(await groupBuilder.GetModelView(group.GroupId));
                }
        }

        private async Task AppendInvites(UserModelView modelView, User user)
        {
            var inviteBuilder = new InviteBuilder();
            inviteBuilder.ConfigureBuilder(_context);

            if (user?.Invites?.Count > 0)
                foreach (var invite in user.Invites)
                {
                    modelView.Invites.Add(await inviteBuilder.GetModelView(invite.Id));
                }
        }
    }
}
