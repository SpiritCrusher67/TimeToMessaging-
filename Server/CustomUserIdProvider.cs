using Microsoft.AspNetCore.SignalR;

namespace Server
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection) => connection.User?.Identity.Name;
    }
}
