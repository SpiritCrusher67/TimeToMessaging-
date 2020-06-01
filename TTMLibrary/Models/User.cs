using System.Collections.Generic;

namespace TTMLibrary.Models
{
    public class User
    {
        public string Login { get; set; }
        public string Password { get; set; }

        public List<UserGroup> Groups { get; set; }
        public ICollection<Group> CreatedGroups { get; set; }
        public ICollection<Invite> Invites { get; set; }
        public ICollection<UserUser> Friends { get; set; }
        public ICollection<UserUser> Users { get; set; }

        public User()
        {
            Groups = new List<UserGroup>();
            CreatedGroups = new List<Group>();
            Invites = new List<Invite>();
            Friends = new List<UserUser>();
            Users = new List<UserUser>();
        }
    }
}
