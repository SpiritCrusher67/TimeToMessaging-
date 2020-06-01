using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using TTMClient.Models;
using TTMLibrary.Models;

namespace TTMClient.ModelViews
{
    public class GroupViewModel : ViewModelBase
    {
        public GroupModel Group { get; set; }
        public string MessageText { get; set; }
        UserModel user;
        HubConnection connection;
        HttpClient httpClient;
        Action<Group> addToListAction;
        public GroupViewModel(GroupModel group, UserModel user, HubConnection connection, HttpClient client, Action<Group> addToListAction)
        {
            if (group == null)
                Group = new GroupModel(new Group());
            else
                Group = group;

            this.user = user;
            this.connection = connection;
            this.addToListAction = addToListAction;
            httpClient = client;
        }
        public GroupViewModel(GroupModel group, UserModel user, HubConnection connection)
        {
            if (group == null)
                Group = new GroupModel(new Group());
            else
                Group = group;

            this.user = user;
            this.connection = connection;
        }

        public GroupViewModel()
        {
            Group = new GroupModel(new Group());
        }

        public RelayCommand OpenFileCommand
        {
            get => new RelayCommand(() =>
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Filter = "JPEG Images|*.jpg";
                if (fileDialog.ShowDialog().HasValue)
                {
                    Group.ImagePatch = fileDialog.FileName;
                }
            });
        }

        public RelayCommand CreateGroupCommand
        {
            get => new RelayCommand(async () =>
            {
                Random rnd = new Random();
                Group group = new Group
                {
                    Id = Guid.NewGuid(),
                    CreatorId = user.Login,
                    Name = Group.Name,
                    Color = ColorTranslator.ToHtml(Color.FromArgb(1, rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255))),
                    
                };
                var content = new StringContent(JsonConvert.SerializeObject(group));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = httpClient.PostAsync("https://localhost:44347/api/Group", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    await connection.InvokeAsync("AddToGroup", group.Id.ToString());
                    addToListAction.Invoke(group);
                }
            });
        }

        public RelayCommand SendMessageCommand
        {
            get => new RelayCommand(async () =>
            {
                await connection.InvokeAsync("SendMessage", new Message(user.Login, Group.Id) { Date = DateTime.Now, Text = MessageText });
            });
        }
    }
}
