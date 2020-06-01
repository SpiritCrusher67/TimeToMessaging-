using System;

namespace TTMLibrary.Models
{
    public class UserGroup
    {
        public string UserLogin { get; set; }
        public User User { get; set; }

        public Guid GroupId { get; set; }
        public Group Group { get; set; }

        public UserGroup(Guid groupId, string userLogin)
        {
            GroupId = groupId;
            UserLogin = userLogin;

        }

        public UserGroup()
        {

        }
    }
}
