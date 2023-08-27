using Microsoft.AspNetCore.SignalR;
using SignalR.Models;

namespace SignalR
{
    public class ChatHub:Hub
    {
        private readonly IWebHostEnvironment env;
        public ChatHub(IWebHostEnvironment env)
        {
            this.env = env;
        }

        public Task SendMessageToGroup(string groupName, string message, string userName)
        {
            return Clients.Group(groupName).SendAsync("Send", $"{userName}: {message}");
        }

        public async Task AddToGroup(string groupName, string userName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("Send", $"{userName} :  has joined the group {groupName}.");
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has left the group {groupName}.");
        }

        public Task SendPrivateMessage(string user, string message)
        {
            return Clients.User(user).SendAsync("ReceiveMessage", message);
        }
        public async Task Upload(ImageData data, string userName)
        {
            string path = Path.Combine(this.env.WebRootPath, "Images");
            path = Path.Combine(path, data.Filename);
            data.Image = data.Image.Substring(data.Image.LastIndexOf(',') + 1);
            string converted = data.Image.Replace('-', '+');
            converted = converted.Replace('-', '/');
            byte[] buffer = Convert.FromBase64String(converted);
            FileStream fs = new FileStream($"{path}", FileMode.Create);
            fs.Write(buffer, 0, buffer.Length);
            fs.Close();

            if (data.Filename.Contains(".jpg") || data.Filename.Contains(".png") || data.Filename.Contains(".gif") || data.Filename.Contains(".txt") || data.Filename.Contains(".docx"))
            {
                await Clients.All.SendAsync("ReceiveMessage", $"{userName}", $"<a target='_blank' href='/Images/{data.Filename}'><img src='/Images/{data.Filename}' width='40px' class='img-thumbnail circle'/></a>");
            }
            else
            {
                await Clients.All.SendAsync("ReceiveMessage", $"{userName}", $"<a target='_blank' href='/Images/{data.Filename}'><img src='/Images/docs.png' width='40px' class='img-thumbnail circle'/></a>");
            }
        }
    }
}
