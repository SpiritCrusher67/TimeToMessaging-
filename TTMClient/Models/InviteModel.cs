using GalaSoft.MvvmLight.Command;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TTMLibrary.Models;

namespace TTMClient.Models
{
    public class InviteModel
    {
        private Invite invite;
        private HttpClient client;
        private HubConnection connection;
        Action<InviteModel> updateListsAction;

        public InviteModel(Invite invite, HttpClient httpClient, HubConnection hubConnection, Action<InviteModel> updateListsAction)
        {
            this.invite = invite;
            client = httpClient;
            connection = hubConnection;
            this.updateListsAction = updateListsAction;
        }

        public Guid Id { get => invite.Id; set => invite.Id = value; }
        public string SenderLogin { get => invite.SenderLogin; set => invite.SenderLogin = value; }
        public string UserLogin { get => invite.UserLogin; set => invite.UserLogin = value; }
        public string GroupName { get=> invite.Group?.Name; }
        public Guid? GroupId { get => invite.GroupId; }
        public string InviteHeader { get => invite.GroupId == default ? "Приглашение в друзья" : $"Приглашение в чат {GroupName}"; }
        public Invite GetInvite() => invite;

        public RelayCommand AcceptInviteCommand
        {
            get => new RelayCommand(async () =>
            {
                await Update(IviteStatus.Accepted);
            });
        }
        public RelayCommand CancelInviteCommand
        {
            get => new RelayCommand(async () =>
            {
                await Update(IviteStatus.Dismissed);

            });
        }

        private async Task Update(IviteStatus status)
        {
            invite.Status = status;

            var content = new StringContent(JsonConvert.SerializeObject(invite));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = client.PutAsync("https://localhost:44347/api/Invite", content).Result;

            if (response.IsSuccessStatusCode)
            {
                if (GroupId != null && status == IviteStatus.Accepted)
                {
                    response = client.GetAsync($"https://localhost:44347/api/Group/AddUser/{GroupId}/{UserLogin}").Result;
                    response.EnsureSuccessStatusCode();
                    await connection.InvokeAsync("AddToGroup", GroupId.ToString());

                }
                else if (GroupId == null && status ==  IviteStatus.Accepted) 
                {
                    response = await client.GetAsync($"https://localhost:44347/api/User/AddFriend/{UserLogin}/{SenderLogin}");
                    response.EnsureSuccessStatusCode();
                    await connection.InvokeAsync("AddToGroup", GroupId.ToString());
                }
                updateListsAction?.Invoke(this);
            }

        }
    }
}
