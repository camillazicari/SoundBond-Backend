using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SoundBond.Data;
using SoundBond.Models;

namespace SoundBond.Hubs
{
    public class ChatHub : Hub
    {
        private readonly SoundBondDbContext _context;

        public ChatHub(SoundBondDbContext context)
        {
            _context = context;
        }

        private string GenerateGroupName(string user1, string user2)
        {
            return string.Compare(user1, user2) < 0
                ? $"{user1}_{user2}"
                : $"{user2}_{user1}";
        }

        public async Task JoinPrivateChat(string otherUserId)
        {
            var currentUserId = Context.UserIdentifier!;
            var groupName = GenerateGroupName(currentUserId, otherUserId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task SendPrivateMessage(string toUserId, string message)
        {
            var fromUserId = Context.UserIdentifier!;

            if (string.IsNullOrWhiteSpace(message) || fromUserId == toUserId)
                return;

            var groupName = GenerateGroupName(fromUserId, toUserId);

            var newMessage = new Message
            {
                SenderId = fromUserId,
                ReceiverId = toUserId,
                Content = message,
                Timestamp = DateTime.UtcNow,
                Letto = false 
            };

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            await Clients.Group(groupName).SendAsync(
                "ReceivePrivateMessage",
                fromUserId,
                message,
                newMessage.Timestamp.ToString("o"),
                false 
            );
        }

        public async Task SegnaComeLetti(string chatWithUserId)
        {
            try
            {
                var currentUserId = Context.UserIdentifier!;

                var messaggiDaSegnare = await _context.Messages
                    .Where(m => m.SenderId == chatWithUserId &&
                               m.ReceiverId == currentUserId &&
                               !m.Letto)
                    .ToListAsync();


                if (messaggiDaSegnare.Any())
                {
                    foreach (var msg in messaggiDaSegnare)
                    {
                        msg.Letto = true;
                    }

                    await _context.SaveChangesAsync();

                    await Clients.User(chatWithUserId).SendAsync("MessaggiLetti", currentUserId);
                    await Clients.User(currentUserId).SendAsync("MessaggiLetti", chatWithUserId);

                }
                
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task LeavePrivateChat(string userId)
        {
            try
            {
                var currentUserId = Context.UserIdentifier!;

                string groupName = GetPrivateChatGroupName(currentUserId, userId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string GetPrivateChatGroupName(string userId1, string userId2)
        {
            var orderedUserIds = new[] { userId1, userId2 }.OrderBy(id => id);
            return $"private-{string.Join("-", orderedUserIds)}";
        }


        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Connesso: {Context.UserIdentifier}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Disconnesso: {Context.UserIdentifier}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}