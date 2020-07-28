using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class User
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool IsConfirmed { get; set; }

        public ICollection<UserUser> Users1 { get; set; }
        public ICollection<UserUser> Users2 { get; set; }

        public ICollection<UserGroup> Groups { get; set; }
        public ICollection<Group> CreatedGroups { get; set; }
        public ICollection<Invite> Invites { get; set; }
        public ICollection<Invite> SendedInvites { get; set; }

        public User()
        {
            Groups = new List<UserGroup>();
            CreatedGroups = new List<Group>();
            Invites = new List<Invite>();
        }
    }
}
