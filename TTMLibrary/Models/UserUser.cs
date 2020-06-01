namespace TTMLibrary.Models
{
    public class UserUser
    {

        public string UserId { get; set; }
        public User User { get; set; }

        public string FriendId { get; set; }
        public User Friend{ get; set; }
    }
}
