using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
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
        
        public ObservableCollection<UserModel> Users { get; set; }

        private UserModel selectedUser;

        public UserModel SelectedUser
        {
            get { return selectedUser; }
            set 
            { 
                selectedUser = value;

                if (selectedUser != default)
                {
                    Invite invite = new Invite
                    {
                        SenderLogin = user.Login,
                        UserLogin = selectedUser.Login,
                        GroupId = Group.Id
                    };
                    SendInvite(invite);
                }

                RaisePropertyChanged(); 
            }
        }


        public GroupViewModel(GroupModel group, UserModel user, HubConnection connection, HttpClient client, Action<Group> addToListAction, ICollection<UserModel> friends)
        {
            if (group == null)
                Group = new GroupModel(new Group());
            else
                Group = group;

            this.user = user;
            this.connection = connection;
            this.addToListAction = addToListAction;
            httpClient = client;
            Users = new ObservableCollection<UserModel>();

            LoadUsers(friends);
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

        async Task<List<string>> GetGroupUsersLogins() //получение списка юзеров
        {
            var response = httpClient.GetAsync("https://localhost:44347/api/Group/UsersList" + Group.Id).Result;
            if (response != null)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<string>>(jsonString);
            }
            return null;
        }

        async Task SendInvite(Invite invite) 
        {
            var content = new StringContent(JsonConvert.SerializeObject(invite));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = httpClient.PostAsync("https://localhost:44347/api/Invite", content).Result;
            if (response.IsSuccessStatusCode)
            {
                await connection.InvokeAsync("SendInviteToGroup",invite);
                Users.Remove(Users.Where(u => u.Login == invite.UserLogin).FirstOrDefault());
            }
        }

        async Task LoadUsers(ICollection<UserModel> friends)
        {
            var logins = await GetGroupUsersLogins();
            var users = new List<User>();
            if (logins != null)
            {
                await Task.Run(() =>
                {
                    foreach (var login in logins)
                    {
                        var response = httpClient.GetAsync($"https://localhost:44347/api/User/{user.Login}/{login}").Result;
                        var jsonObj = response.Content.ReadAsStringAsync().Result;
                        users.Add(JsonConvert.DeserializeObject<User>(jsonObj));
                    }
                });

                foreach (var user in users)
                {
                    if (friends.Where(u => u.Login != user.Login).FirstOrDefault() != null)
                    {
                        Users.Add(new UserModel(user,httpClient,connection,user.Login));

                    }
                }

            }
            else
            {
                foreach (var friend  in friends)
                {
                    Users.Add(friend);
                }
            }
        
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
