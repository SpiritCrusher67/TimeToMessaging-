using Microsoft.EntityFrameworkCore;
using Server.Models;
using Server.Services.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTMLibrary.ModelViews;

namespace Server.Services
{
    public class InviteService : IEntityService<Invite, InviteModelView>
    {
        ApplicationContext _context;

        public InviteService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<InviteModelView> Create(InviteModelView modelView, IEntityBuilder<Invite, InviteModelView> builder)
        {
            builder.ConfigureBuilder(_context);

            var invite = await builder.Create(modelView);

            if (invite == null)
                return null;

            await _context.Invites.AddAsync(invite);
            await _context.SaveChangesAsync();

            modelView.Id = invite.Id;

            return modelView;
        }

        public async Task<bool> Delete(object id, string userLogin)
        {
            var invite = await _context.Invites.FindAsync(id);

            if (invite == null || invite.UserLogin != userLogin)
                return false;

            _context.Invites.Remove(invite);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<InviteModelView> Get(object Id, IEntityBuilder<Invite, InviteModelView> builder)
        {
            builder.ConfigureBuilder(_context);

            return await builder.GetModelView(Id);
        }

        public Task<InviteModelView> Update(InviteModelView modelView)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> Accept(Guid inviteId, string userLogin)
        {
            var invite = await _context.Invites.FindAsync(inviteId);

            if (invite == null || invite.UserLogin != userLogin)
                return false;

            var user = await _context.Users.Include("Users1").Include("Users2").Include("Groups").Where(u => u.Login == invite.UserLogin).FirstOrDefaultAsync();

            if (invite.GroupId != null)           
                user.Groups.Add(new UserGroup(invite.GroupId.Value, user.Login));
            else
            {
                user.Users1.Add(new UserUser { FriendId = invite.SenderLogin, UserId = user.Login });
                user.Users2.Add(new UserUser { FriendId = user.Login, UserId = invite.SenderLogin });
            }

            await _context.SaveChangesAsync();

            return true;
        }

    }
}
