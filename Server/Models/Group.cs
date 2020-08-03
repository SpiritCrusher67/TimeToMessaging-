using System;
using System.Collections.Generic;

namespace Server.Models
{
    public class Group
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string CreatorId{ get; set; }
        public User Creator { get; set; }

        public ICollection<UserGroup> Users { get; set; }
        public ICollection<Invite> Invites { get; set; }

    }
}
