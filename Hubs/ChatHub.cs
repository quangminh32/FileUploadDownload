using Microsoft.AspNetCore.SignalR;

namespace FileUploadDownload.Hubs
{
    public class ChatHub : Hub
    {
        private static Dictionary<string, string> userConnections = new Dictionary<string, string>();

        public async Task SendMessage(string user, string message)
        {
            // Lưu connection ID của user
            if (!userConnections.ContainsKey(user))
            {
                userConnections[user] = Context.ConnectionId;
            }
            else
            {
                userConnections[user] = Context.ConnectionId;
            }

            // Gửi tin nhắn đến tất cả admin (có thể mở rộng để lưu danh sách admin)
            await Clients.All.SendAsync("ReceiveMessage", user, message, "");
        }

        public async Task SendMessageWithImage(string user, string message, string imageUrl)
        {
            // Lưu connection ID của user
            if (!userConnections.ContainsKey(user))
            {
                userConnections[user] = Context.ConnectionId;
            }
            else
            {
                userConnections[user] = Context.ConnectionId;
            }

            // Gửi tin nhắn kèm ảnh đến tất cả admin
            await Clients.All.SendAsync("ReceiveMessage", user, message, imageUrl);
        }

        public async Task SendAdminReply(string targetUser, string adminName, string message)
        {
            // Gửi reply từ admin đến user cụ thể
            if (userConnections.ContainsKey(targetUser))
            {
                await Clients.Client(userConnections[targetUser]).SendAsync("ReceiveAdminReply", adminName, message);
            }
            
            // Gửi reply đến tất cả admin để hiển thị trong admin panel
            await Clients.All.SendAsync("ReceiveAdminReply", adminName, message);
        }

        public async Task SendAdminReplyWithImage(string targetUser, string adminName, string message, string imageUrl)
        {
            // Gửi reply kèm ảnh từ admin đến user cụ thể
            if (userConnections.ContainsKey(targetUser))
            {
                await Clients.Client(userConnections[targetUser]).SendAsync("ReceiveAdminReply", adminName, message, imageUrl);
            }
            
            // Gửi reply kèm ảnh đến tất cả admin để hiển thị trong admin panel
            await Clients.All.SendAsync("ReceiveAdminReply", adminName, message, imageUrl);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Xóa user khỏi danh sách khi disconnect
            var userToRemove = userConnections.FirstOrDefault(x => x.Value == Context.ConnectionId);
            if (!string.IsNullOrEmpty(userToRemove.Key))
            {
                userConnections.Remove(userToRemove.Key);
                await Clients.All.SendAsync("UserDisconnected", userToRemove.Key);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}