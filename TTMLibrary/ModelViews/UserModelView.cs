using System;
using System.Collections.Generic;
using System.Text;

namespace TTMLibrary.ModelViews
{
    public class UserModelView
    {
        public string Login { get; set; }
        public byte[] Avatar { get; set; }
        public ICollection<UserModelView> Friends { get; set; }
        public ICollection<GroupModelView> Groups { get; set; }
        public ICollection<InviteModelView> Invites { get; set; }

        public UserModelView(string login)
        {
            Login = login;

            Friends = new List<UserModelView>();
            Groups = new List<GroupModelView>();
            Invites = new List<InviteModelView>();
        }

    }
}
