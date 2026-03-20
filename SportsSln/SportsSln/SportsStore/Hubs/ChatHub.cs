using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class ChatHub : Hub
{
    private readonly IBotService _botService;

    public ChatHub(IBotService botService)
    {
        _botService = botService;
    }

    public async Task SendMessage(string user, string message)
    {
        // Gửi tin nhắn tới tất cả client
        await Clients.All.SendAsync("ReceiveMessage", user, message);

        // Kiểm tra bot trả lời (chỉ cho khách)
        if (user != "Admin")
        {
            string botReply = _botService.GetReply(message);
            if (!string.IsNullOrEmpty(botReply))
            {
                await Clients.All.SendAsync("ReceiveMessage", "Bot", botReply);
            }
        }
    }
}
