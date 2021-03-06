﻿using System;
using System.Collections.Generic;

namespace Server.Models
{
    public class Message
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public Guid GroupId { get; set; }
        public Group Group { get; set; }

        public DateTime Date { get; set; }
        public string Text { get; set; }
        public string AttachedFileName { get; set; }

        public Message()
        {
             
        }

 
    }
}
