using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTMLibrary.ModelViews;

namespace Server.Services.Builders
{
    public class InviteBuilder : IEntityBuilder<Invite, InviteModelView>
    {
        ApplicationContext _context;

        public void ConfigureBuilder(ApplicationContext context, Microsoft.AspNetCore.Hosting.IWebHostEnvironment environment = null)
        {
            _context = context;
        }

        public Task<Invite> Create(InviteModelView modelView)
        {
            return Task.FromResult(new Invite
            {
                Id = Guid.NewGuid(),
                SenderLogin = modelView.SenderLogin,
                UserLogin = modelView.ReceiverLogin,
                GroupId = modelView.GroupId
            });
        }

        public async Task<Invite> GetEntity(object id)
        {
            return await _context.Invites.FindAsync(id);
        }

        public async Task<InviteModelView> GetModelView(object id)
        {
            var invite = await _context.Invites.FindAsync(id);

            if (invite == null)
                return null;

            return new InviteModelView
            {
                Id = invite.Id,
                GroupId = invite.GroupId,
                SenderLogin = invite.SenderLogin,
                ReceiverLogin = invite.UserLogin
            };

        }
    }
}
