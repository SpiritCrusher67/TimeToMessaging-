using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TTMClient.Models;
using TTMClient.UserControls;
using TTMClient.UserControls.Lists;
using TTMLibrary.Models;


namespace TTMClient.ModelViews
{
    public class MainModelView : ViewModelBase
    {
        private GroupModel selectedGroup;
        HttpClient client; //клиент для запросов к апи
        private UserControl displayableControl;
        private UserControl displayableListControl;
        private UserModel selectedFriend;

        private List<UserModel> friendsTmp;

        HubConnection connection; //подключение к хабу
        string token;
        private bool chatsIsSelected;
        private bool friendsIsSelected;
        private bool invitesIsSelected;

        public string SearchText { get; set; }

        public UserModel SelectedFriend
        {
            get => selectedFriend; 
            set
            { 
                selectedFriend = value;
                if (selectedFriend != null)
                {
                    if (!(DisplayableControl is UserControls.UserProfileControl))
                    {
                        DisplayableControl = new UserProfileControl();
                        
                    }
                    DisplayableControl.DataContext = selectedFriend;
                }
                RaisePropertyChanged();
            }
        }

        public GroupModel SelectedGroup //выбранная группа
        {
            get => selectedGroup;
            set
            {
                selectedGroup = value;
                if (selectedGroup != null)
                {
                    if (!(DisplayableControl is ChatControl))
                    {
                        DisplayableControl = new ChatControl();
                    }
                    DisplayableControl.DataContext = new GroupViewModel(selectedGroup, User, connection, client, group => Groups.Add(new GroupModel(group)), friendsTmp);

                }
                RaisePropertyChanged();
            }
        }
        public UserControl DisplayableControl //отображаемый контрол
        {
            get => displayableControl;
            set
            {
                displayableControl = value;
                RaisePropertyChanged();
            }
        }

        public UserControl DisplayableListControl
        {
            get
            {
                return displayableListControl;
            }
            set
            {
                displayableListControl = value;
                RaisePropertyChanged();
            }
        }

        public bool ChatsIsSelected
        {
            get => chatsIsSelected;
            set
            {
                chatsIsSelected = value;
                if (chatsIsSelected)
                {
                    DisplayableListControl = new UserControls.Lists.ChatListControl();
                    DisplayableListControl.DataContext = this;
                }
            }
        }
        public bool FriendsIsSelected
        {
            get => friendsIsSelected;
            set
            {
                friendsIsSelected = value;
                if (friendsIsSelected)
                {
                    DisplayableListControl = new UserControls.Lists.FriendsListControl();
                    DisplayableListControl.DataContext = this;
                }
            }
        }
        public bool InvitesIsSelected 
        { 
            get => invitesIsSelected;
            set
            {
                invitesIsSelected = value;
                if (invitesIsSelected)
                {
                    DisplayableListControl = new UserControls.Lists.InvitesListControl();
                    DisplayableListControl.DataContext = this;
                }
            }
        }

        public UserModel User { get; set; } //авторизованный юзер

        public ObservableCollection<GroupModel> Groups { get; set; }
        public ObservableCollection<UserModel> Friends { get; set; }
        public ObservableCollection<InviteModel> Invites { get; set; }

        public MainModelView(User user)
        {
            User = new UserModel(user,client,connection,user.Login);
            client = new HttpClient();
            Groups = new ObservableCollection<GroupModel>();
            Friends = new ObservableCollection<UserModel>();
            Invites = new ObservableCollection<InviteModel>();
            ChatsIsSelected = true;
            friendsTmp = new List<UserModel>();

            GetToken();
            LoadUserChats();
            LoadUserFriends();
            LoadUserInvites();
            ConnectToHub();
        }

        public async void LoadUserChats() //загрузка чатов
        {
            Groups.Clear();
            var groupIds = await GetUserGroupsIds();
            var groups = new List<Group>();
            if (groupIds != null)
            {
                await Task.Run(() =>
                {
                    foreach (var id in groupIds)
                    {
                        var response = client.GetAsync($"https://localhost:44347/api/Group/{User.Login}/{id}").Result;
                        var jsonObj = response.Content.ReadAsStringAsync().Result;
                        groups.Add(JsonConvert.DeserializeObject<Group>(jsonObj));
                    }
                });
                foreach (var group in groups)
                {
                    Groups.Add(new GroupModel(group));
                }
            }
      
        }
        public async void LoadUserFriends() //загрузка друзей
        {
            Friends.Clear();
            var logins = await GetUserFriendsLogins();
            var users = new List<User>();
            if (logins != null)
            {
                await Task.Run(() =>
                {
                    foreach (var login in logins)
                    {
                        var response = client.GetAsync($"https://localhost:44347/api/User/{User.Login}/{login}").Result;
                        var jsonObj = response.Content.ReadAsStringAsync().Result;
                        users.Add(JsonConvert.DeserializeObject<User>(jsonObj));
                    }
                });

                foreach (var user in users)
                {
                    var userTmp = new UserModel(user, client, connection, User.Login);
                    Friends.Add(userTmp);
                    friendsTmp.Add(userTmp);
                }
            }

        }
        public async void LoadUserInvites() //загрузка приглашений
        {
            var ids = await GetUserInvitesIds();
            var invites = new List<Invite>();
            await Task.Run(() =>
            {
                foreach (var id in ids)
                {
                    var response = client.GetAsync($"https://localhost:44347/api/Invite/{User.Login}/{id}").Result;
                    var jsonObj = response.Content.ReadAsStringAsync().Result;
                    invites.Add(JsonConvert.DeserializeObject<Invite>(jsonObj));
                }
            });

            foreach (var invite in invites)
            {
                Invites.Add(new InviteModel(invite, client,connection,invite => { LoadUserChats(); LoadUserFriends(); Invites.Remove(invite); } ));
            }
        }
        async Task<List<Guid>> GetUserInvitesIds() //получение списка групп юзера
        {
            var response = client.GetAsync("https://localhost:44347/api/Invite/" + User.Login).Result;
            if (response != null)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Guid>>(jsonString);
            }
            return null;
        }
        async Task<List<Guid>> GetUserGroupsIds() //получение списка групп юзера
        {
            var response = client.GetAsync("https://localhost:44347/api/Group/" + User.Login).Result;
            if (response != null)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Guid>>(jsonString);
            }
            return null;
        }
        async Task<List<string>> GetUserFriendsLogins() //получение списка друзей юзера
        {
            var response = client.GetAsync("https://localhost:44347/api/User/" + User.Login).Result;
            if (response != null)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<string>>(jsonString);
            }
            return null;
        }



        #region HubConnection
        async void GetToken()
        {
            string data = $"login={User.Login}&password={User.Password}";
            var content = new StringContent(data);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var response = client.PostAsync("https://localhost:44347/token", content).Result;
            if (response != null)
            {
                token = await response.Content.ReadAsStringAsync();
            }
        }
        async void ConnectToHub()
        {
            connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:44347/hub", (opt) => opt.AccessTokenProvider = () => Task.FromResult(token))
                .Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            connection.On<Message>("ReceiveMessage", (message) =>
            {
                var group = Groups.Where(g => g.Id == message.GroupId).FirstOrDefault();
                var msg = new MessageModel(message);
                group.LastMessage = msg;
                group.Messages.Add(msg);
            });

            connection.On<Invite>("ReceiveInviteToGroup", (invite) =>
            {

            });

            try
            {
                await connection.StartAsync();
            }
            catch
            {

            }

            foreach (var group in Groups)
            {
                await connection.InvokeAsync("AddToGroup", group.Id.ToString());
            }
        }
        #endregion

        #region Commands
        public RelayCommand CreateGroupCommand
        {
            get => new RelayCommand(() =>
            {
                DisplayableControl = new GroupControl();
                DisplayableControl.DataContext = new GroupViewModel(null, User, connection, client, group => Groups.Add(new GroupModel(group)),Friends);
            });
        }
        public RelayCommand SearchCommand
        {
            get => new RelayCommand(() =>
            {
                if (FriendsIsSelected)
                {
                    if (SearchText != string.Empty)
                    {
                        var friends = friendsTmp.Where(f => f.Login == SearchText);
                        if (friends.Count() > 0)
                        {
                            Friends.Clear();
                            foreach (var item in friends)
                            {
                                Friends.Add(item);
                            }
                        }
                        else
                        {
                            var response = client.GetAsync($"https://localhost:44347/api/User/Search/{SearchText}").Result;
                            var jsonObj = response.Content.ReadAsStringAsync().Result;
                            var list = (JsonConvert.DeserializeObject<List<User>>(jsonObj));

                            if (list.Count > 0)
                            {
                                Friends.Clear();

                                foreach (var item in list)
                                {
                                    Friends.Add(new UserModel(item,client,connection,User.Login));
                                }
                            }
                        }
                    }
                    else
                    {
                        Friends.Clear();
                        foreach (var item in friendsTmp)
                        {
                            Friends.Add(item);
                        }
                    }
                    
                }
            });
        }
        #endregion
    }
}
