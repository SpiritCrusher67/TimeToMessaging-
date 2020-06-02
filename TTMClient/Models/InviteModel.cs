using System;
using System.Collections.Generic;
using System.Text;
using TTMLibrary.Models;

namespace TTMClient.Models
{
    public class InviteModel
    {
        private Invite invite;

        public InviteModel(Invite invite)
        {
            this.invite = invite;

        }

        public Guid Id { get => invite.Id; set => invite.Id = value; }
        public string SenderLogin { get => invite.SenderLogin; set => invite.SenderLogin = value; }
        public string GroupName { get=> invite.Group?.Name; }
        public string InviteHeader { get => invite.GroupId == default ? "Приглашение в друзья" : $"Приглашение в чат {GroupName}"; }
        public Invite GetInvite() => invite;
    }
}
