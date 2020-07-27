using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTMLibrary.Models;

namespace Server.Models
{
    public class UserModel : TTMLibrary.Models.User 
    {
        public string Password { get; set; }
        public bool IsConfirmed { get; set; }

        public ICollection<UserUser> Users1 { get; set; }
        public ICollection<UserUser> Users2 { get; set; }

        public UserModel()
        {
            Users1 = new List<UserUser>();
            Users2 = new List<UserUser>();
        }
    }
}
