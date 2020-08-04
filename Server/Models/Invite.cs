using System;
using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class Invite
    {
        public Guid Id { get; set; }

        public IviteStatus Status { get; set; }

        public string SenderLogin { get; set; }
        public User Sender { get; set; }

        public string UserLogin { get; set; }
        public User User { get; set; }

        public Guid? GroupId { get; set; }
        public Group Group { get; set; }
    }

    public enum IviteStatus
    {
        Sended,
        Accepted,
        Dismissed
    }
}
