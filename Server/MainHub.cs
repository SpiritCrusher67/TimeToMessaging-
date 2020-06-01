﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTMLibrary.Models;

namespace Server
{
    [Authorize]
    public class MainHub : Hub
    {
        public async Task SendMessage(Message message)
        {
            await Clients.Group(message.GroupId.ToString()).SendAsync("ReceiveMessage", message);

        }

        public async Task AddToGroup(string groupId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId,groupId);
        }

        public async Task SendInviteToGroup(string userLogin, Invite invite)
        {
            await Clients.User(userLogin).SendAsync("ReceiveInviteToGroup", invite);
        }

    }
}
