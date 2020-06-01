using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TTMLibrary.Models;

namespace TTMClient.Models
{
    public class UserModel : ObservableObject
    {
        private User user;
        public UserModel(User user)
        {
            this.user = user;
        }

        public string Login { get => user.Login; }
        public string Password { get => user.Password; set => user.Password = value; }
       
        
        
    }
}
