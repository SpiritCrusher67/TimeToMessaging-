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
using TTMLibrary.Models;
using Xunit;

namespace TimeToMessaging.Tests
{
    public class UserControllerTests
    {
        private IEnumerable<UserModel> GetTestUsers() => new List<UserModel>
        {
            new UserModel { Login = "test1",Password = "1" }, 
            new UserModel { Login = "test2",Password = "1" }, 
            new UserModel { Login = "test3",Password = "1" }, 
            new UserModel { Login = "test4",Password = "1" }
        };

        private UserController GetUserAuthorizedController()
        {
            var dbContext = new Mock<ApplicationContext>();
            var dbSetMock = GetTestUsers().AsQueryable().BuildMockDbSet();
            dbContext.SetupGet(c => c.Users).Returns(dbSetMock.Object);
            var enviromentMock = new Mock<IWebHostEnvironment>();
            enviromentMock.SetupGet(e => e.WebRootPath).Returns(AppDomain.CurrentDomain.BaseDirectory + "/wwwroot");
            var userService = new UsersService(dbContext.Object,enviromentMock.Object);
            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(c => c.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType,"test1")
            })));
            var controllerContext = new ControllerContext() { HttpContext = httpContext.Object };
            var controller = new UserController(userService);
            controller.ControllerContext = controllerContext;

            return controller;
        }

        private long GetDefaultAvatarLenght() => File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "/wwwroot/Files/DefaultFiles/DefaultAvatar.png").Length;

        [Fact]
        public void GetUserFullData_Test()
        {
            var controller = GetUserAuthorizedController();

            var result = (controller.GetUserData("test1").Result as OkObjectResult).Value as User;

            Assert.NotNull(result);
            Assert.Equal("test1", result.Login);
            Assert.NotEmpty(result.Friends);
        }

        [Fact]
        public void GetUserData_Test()
        {
            var controller = GetUserAuthorizedController();

            var result = (controller.GetUserData("test2").Result as OkObjectResult).Value as User;

            Assert.NotNull(result);
            Assert.Equal("test2", result.Login);
            Assert.Empty(result.Friends);
        }

        [Fact]
        public void GetUserData_Contains_User_Avatar_Test()
        {
            var controller = GetUserAuthorizedController();
            var defaultAvatartLenght = GetDefaultAvatarLenght();

            var result = (controller.GetUserData("test1").Result as OkObjectResult).Value as User;

            Assert.NotNull(result);
            Assert.NotEmpty(result.Avatar);
            Assert.NotEqual(defaultAvatartLenght, result.Avatar.Length);
        }

        [Fact]
        public void GetUserData_Contains_Default_Avatar_Test()
        {
            var controller = GetUserAuthorizedController();
            var defaultAvatartLenght = GetDefaultAvatarLenght();

            var result = (controller.GetUserData("test2").Result as OkObjectResult).Value as User;

            Assert.NotNull(result);
            Assert.NotEmpty(result.Avatar);
            Assert.Equal(defaultAvatartLenght, result.Avatar.Length);
        }
    }
}
