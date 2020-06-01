using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;
using TTMLibrary.Models;

namespace TTMClient.Models
{
    public class MessageModel
    {
        private Message message;
        public string SenderLogin { get => message.UserId;}
        public Guid GroupId { get => message.GroupId; }
        public DateTime Date { get => message.Date;}
        public string TimeStr 
        { 
            get
            {
                string date = message.Date.Day == DateTime.Now.Day ? 
                    "Сегодня" : message.Date.Day == DateTime.Now.AddDays(-1).Day ? 
                    "Вчера" : message.Date.ToString("D");
                return date + " в " + message.Date.ToString("t");
            }
        }

        public string ShortTimeStr { get => message.Date.ToString("t");  }

        public string Text { get => message.Text;  }

        public MessageModel(object messageObj)
        {
            message = messageObj as Message;

            if (message == null)
            {
                throw new NullReferenceException();
            }
        }

        public override string ToString()
        {
            return $"{SenderLogin}: {Text}";
        }
    }
}
