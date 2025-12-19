using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace DoAnChamSocSucKhoe.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string conversationId, string message)
        {
            await Clients.Group(conversationId).SendAsync("ReceiveMessage", Context.User?.Identity?.Name ?? "Unknown", message);
        }

        public async Task JoinGroup(string conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
        }

        public async Task LeaveGroup(string conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
        }

        public async Task TypingIndicator(string conversationId, bool isTyping)
        {
            await Clients.OthersInGroup(conversationId).SendAsync("UserTyping", Context.User?.Identity?.Name ?? "Unknown", isTyping);
        }
    }
}