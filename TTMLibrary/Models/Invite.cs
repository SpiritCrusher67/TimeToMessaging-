using System;

namespace TTMLibrary.Models
{
    public class Invite
    {
        public Guid Id { get; set; }

        public IviteStatus Status { get; set; }

        public string UserLogin { get; set; }
        public User User { get; set; }

        public Guid GroupId { get; set; }
        public Group Group { get; set; }

        public Invite()
        {

        }

        public Invite(string userLogin, Guid groupId)
        {
            UserLogin = userLogin;
            GroupId = groupId;
        }
    }

    public enum IviteStatus
    {
        Sended,
        Accepted,
        Dismissed
    }
}
