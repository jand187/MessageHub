using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace MessageHub
{
    public class DefaultHub : Hub
    {
        const string appName = "Server DefaultHub";


        public async Task Broadcast(string sender, string message)
        {
            var senderDecorated = sender + " [" + Context.ConnectionId+ "]";
            await Clients.All.SendAsync("ReceiveMessage", senderDecorated, message);
        }

        public async Task AddToGroup(string sender, string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var senderDecorated = sender + " [" + Context.ConnectionId+ "] joined group: " + groupName;
            
            await Clients.Group(groupName).SendAsync("ReceiveMessage", appName, senderDecorated);
        }

        public async Task SendGroup(string sender, string groupName, string message)
        {
            var senderDecorated = sender + " [" + Context.ConnectionId+ "]";
            await Clients.Group(groupName).SendAsync("ReceiveMessage", senderDecorated, message);
        }

        public override async Task OnConnectedAsync()
        {
            var connection = "[" + Context.ConnectionId+ "] Client connected";
            await Clients.Caller.SendAsync("ReceiveMessage", appName, connection);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connection = "[" + Context.ConnectionId+ "] Client disconnected";
            await Clients.Caller.SendAsync("ReceiveMessage", appName, connection);
        }
    }
}