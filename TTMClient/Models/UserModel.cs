using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TTMLibrary.Models;

namespace TTMClient.Models
{
    public class UserModel : ObservableObject
    {
        private User user;
        HttpClient client;
        HubConnection connection;
        string senderLogin;
        public UserModel(User user,HttpClient httpClient, HubConnection hubConnection, string senderLogin)
        {
            this.user = user;
            client = httpClient;
            connection = hubConnection;
            this.senderLogin = senderLogin;
        }

        public string Login { get => user.Login; }
        public string Password { get => ""; set => value = ""; }
        public RelayCommand SendInviteCommand
        {
            get => new RelayCommand(async () =>
            {
                await SendInvite();
            });
        }

        async Task SendInvite()
        {
            Invite invite = new Invite 
            {
                SenderLogin = senderLogin,
                UserLogin = Login,
            };

            var content = new StringContent(JsonConvert.SerializeObject(invite));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = client.PostAsync("https://localhost:44347/api/Invite", content).Result;
            if (response.IsSuccessStatusCode)
            {
                await connection.InvokeAsync("SendInviteToGroup", invite);
            }
        }
    }
}
