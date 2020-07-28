using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using Server.Controllers;
using Server.Models;
using Server.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using Xunit;
using TTMLibrary.ModelViews;
using TimeToMessaging.Tests.TestingExtensions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TimeToMessaging.Tests
{
    public class UserControllerTests
    {
        private List<User> _testUsers;

        public UserControllerTests()
        {
            _testUsers = new List<User>
            {
                new User { Login = "test1",Password = "1" },
                new User { Login = "test2",Password = "1" },
                new User { Login = "test3",Password = "1",
                    Users1 = new List<UserUser>
                    {
                        new UserUser { FriendId = "test4", UserId = "test3" }
                    },
                    Users2 = new List<UserUser>
                    {
                        new UserUser { FriendId = "test3", UserId = "test4" }
                    }},
                new User { Login = "test4",Password = "1",
                    Users1 = new List<UserUser>
                    {
                        new UserUser { FriendId = "test3", UserId = "test4" }
                    },
                    Users2 = new List<UserUser>
                    {
                        new UserUser { FriendId = "test4", UserId = "test3" }
                    }
                }
            };
        }

        private Mock<ApplicationContext> GetMockContext()
        {
            var dbContext = new Mock<ApplicationContext>();
            var usersMock = _testUsers.AsQueryable().BuildMockDbSet();
            dbContext.SetupGet(c => c.Users).Returns(usersMock.Object);

            return dbContext;
        }

        private Mock<IWebHostEnvironment> GetMockEnvironment()
        {
            var enviromentMock = new Mock<IWebHostEnvironment>();
            enviromentMock.SetupGet(e => e.WebRootPath).Returns(AppDomain.CurrentDomain.BaseDirectory + "/TestFiles");

            return enviromentMock;
        }

        private UserController GetUserController()
        {
            var userService = new UsersService(GetMockContext().Object,GetMockEnvironment().Object);
            var controller = new UserController(userService);

            return controller;
        }

        private long GetDefaultAvatarLenght() => File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "/TestFiles/Files/DefaultFiles/DefaultAvatar.png").Length;

        [Fact]
        public void GetUserData_Test()
        {
            var controller = GetUserController();

            var result = controller.GetUserData("test1").Result;
            var modelView = (result as OkObjectResult).Value as UserModelView;

            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(modelView);
            Assert.Equal("test1", modelView.Login);
        }

        [Fact]
        public void GetUserData_Contains_Friends_Test()
        {
            var controller = GetUserController().MockAuthorizeAs("test3");

            var result = (controller.GetFullUserData().Result as OkObjectResult).Value as UserModelView;

            Assert.NotNull(result);
            Assert.Equal("test3", result.Login);
            Assert.NotEmpty(result.Friends);
            Assert.Equal("test4", result.Friends.FirstOrDefault().Login);
        }

        [Fact]
        public void GetUserData_Contains_User_Avatar_Test()
        {
            var controller = GetUserController();

            var defaultAvatartLenght = GetDefaultAvatarLenght();

            var result = (controller.GetUserData("test1").Result as OkObjectResult).Value as UserModelView;

            Assert.NotNull(result);
            Assert.NotEmpty(result.Avatar);
            Assert.NotEqual(defaultAvatartLenght, result.Avatar.Length);
        }

        [Fact]
        public void GetUserData_Contains_Default_Avatar_Test()
        {
            var controller = GetUserController();

            var defaultAvatartLenght = GetDefaultAvatarLenght();

            var result = (controller.GetUserData("test2").Result as OkObjectResult).Value as UserModelView;

            Assert.NotNull(result);
            Assert.NotEmpty(result.Avatar);
            Assert.Equal(defaultAvatartLenght, result.Avatar.Length);
        }

        [Fact]
        public void CreateUserData_Witch_Valid_Data_Test()
        {
            var controller = GetUserController();

            var modelView = new RegistrationModelView
            {
                Login = "test10",
                Password = "12345567",
                ConfirmPassword = "12345567",
                Email = "test@gg.gg"
            };

            var response = controller.CreateUser(modelView).Result;

            Assert.IsType<OkResult>(response);
        }
    }
}
