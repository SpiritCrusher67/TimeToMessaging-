

namespace Server.Models
{
    public class UserUser
    {

        public string UserId { get; set; }
        public UserModel User { get; set; }

        public string FriendId { get; set; }
        public UserModel Friend { get; set; }
    }
}
