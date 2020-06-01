using System;
using System.Collections.Generic;

namespace TTMLibrary.Models
{
    [Serializable]
    public class Message
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public Guid GroupId { get; set; }
        public Group Group { get; set; }

        public DateTime Date { get; set; }

        public string Text { get; set; }
        //[NotMapped]
        //public ICollection<byte[]> AttachedFiles { get; set; }
        //[NotMapped]
        //public ICollection<string> AttachedFilesExtensions { get; set; }

        public Message(string userId, Guid groupId)
        {
            UserId = userId;
            GroupId = groupId;
        }

        public Message()
        {
             
        }

 
    }
}
