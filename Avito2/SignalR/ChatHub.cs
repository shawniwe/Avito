using System.Security.Claims;
using Avito2.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Avito2.SignalR
{
    public class ChatHub : Hub
    {
        public async Task Send(string receiver, string sender, string message)
        {
            //var user = Clients.User(receiver);
            //await user.SendAsync("Send", sender, message);
            await Clients.Group(receiver).SendAsync("Send", sender, message);
        }

        public void Register(string user)
        {
            Groups.AddToGroupAsync(Context.ConnectionId, user);
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
