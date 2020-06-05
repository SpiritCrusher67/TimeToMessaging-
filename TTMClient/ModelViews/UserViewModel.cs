using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TTMLibrary.Models;

namespace TTMClient.ModelViews
{
    class UserViewModel : ViewModelBase
    {
        public User User { get; set; }
        public string SecondPassword { get; set; }
        HttpClient client;

        public UserViewModel()
        {
            User = new User();
            client = new HttpClient();
        }

        async Task<string> GetToken()
        {
            string token = string.Empty;
            string data = $"login={User.Login}&password={User.Password}";
            var content = new StringContent(data);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var response = client.PostAsync("https://localhost:44347/token", content).Result;
            if (response != null)
            {
                token = await response.Content.ReadAsStringAsync();
            }
            return token;
        }

        async Task CreateAccount()
        {
            var content = new StringContent(JsonConvert.SerializeObject(User));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync("https://localhost:44347/api/User", content);
            if (response.IsSuccessStatusCode)
            {
                CancelCommand.Execute(null);
            }
        }

        public RelayCommand LoginCommand
        {
            get => new RelayCommand(async () =>
            {
                var result = await GetToken();
                if (result != string.Empty)
                {
                    var window = Application.Current.MainWindow = new MainWindow();
                    window.DataContext = new ModelViews.MainModelView(User);
                    window.Show();
                    Application.Current.Windows[0].Close();
                }
            },
                () => User.Login != string.Empty && User.Password != string.Empty);
        }

        public RelayCommand SinginCommand
        {
            get => new RelayCommand(() =>
            {
                var window = Application.Current.MainWindow = new RegistrationWindow();
                window.DataContext = new ModelViews.UserViewModel();
                window.Show();
                Application.Current.Windows[0].Close();
            });
        }

        public RelayCommand CreateAccountCommand
        {
            get => new RelayCommand(async () =>
            {
                await CreateAccount();
            },
                () => User.Login != string.Empty && User.Password != string.Empty && User.Password == SecondPassword);
        }
        public RelayCommand CancelCommand
        {
            get => new RelayCommand(() =>
            {
                var window = Application.Current.MainWindow = new LoginWindow();
                window.DataContext = new ModelViews.UserViewModel();
                window.Show();
                Application.Current.Windows[0].Close();
            });
        }
    }
}
