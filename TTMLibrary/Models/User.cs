using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TTMLibrary.Models
{
    public class User
    {
        public string Login { get; set; }
        public string Email { get; set; }
        [NotMapped]
        public byte[] Avatar { get; set; }

        public List<UserGroup> Groups { get; set; }
        public ICollection<Group> CreatedGroups { get; set; }
        [NotMapped]
        public ICollection<User> Friends { get; set; }
        public ICollection<Invite> Invites { get; set; }
        public ICollection<Invite> SendedInvites { get; set; }

        public User()
        {
            Groups = new List<UserGroup>();
            CreatedGroups = new List<Group>();
            Invites = new List<Invite>();
            Friends = new List<User>();
        }
    }
}
