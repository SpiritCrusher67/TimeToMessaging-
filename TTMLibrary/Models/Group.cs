using System;
using System.Collections.Generic;

namespace TTMLibrary.Models
{
    public class Group
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string CreatorId{ get; set; }
        public User Creator { get; set; }
        public string Color { get; set; }

        public List<UserGroup> Users { get; set; }
        public ICollection<Invite> Invites { get; set; }

        public Group()
        {
            Users = new List<UserGroup>();

        }
        public Group(string creatorLogin, string name) : base()
        {
            CreatorId = creatorLogin;
            Name = name;
            Invites = new List<Invite>();
        }

    }
}
