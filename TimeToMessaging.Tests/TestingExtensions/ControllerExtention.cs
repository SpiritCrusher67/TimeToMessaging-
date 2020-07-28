using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace TimeToMessaging.Tests.TestingExtensions
{
    public static class ControllerExtention
    {
        public static T MockAuthorizeAs<T>(this T controller, string userLogin) where T : ControllerBase
        {
            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(c => c.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userLogin)
            })));

            var controllerContext = new ControllerContext() { HttpContext = httpContext.Object };
            controller.ControllerContext = controllerContext;

            return controller;
        }
    }
}
