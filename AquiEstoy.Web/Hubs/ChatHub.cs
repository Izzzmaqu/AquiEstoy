using Microsoft.AspNetCore.SignalR;

namespace AquiEstoy.Web.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message, string casoId)
        {
            await Clients.Group(casoId).SendAsync("ReceiveMessage", user, message, DateTime.Now.ToString("HH:mm"));
        }

        public async Task JoinCaseGroup(string casoId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, casoId);
        }
    }
}