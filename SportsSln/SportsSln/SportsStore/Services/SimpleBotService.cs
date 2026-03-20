public interface IBotService
{
    string GetReply(string message);
}

public class SimpleBotService : IBotService
{
    private readonly Dictionary<string, string> _responses = new()
    {
        {"xin chào", "Chào bạn! Mình là trợ lý ảo."},
        {"giá sản phẩm", "Bạn có thể xem giá ở trang chi tiết sản phẩm."},
        {"hỗ trợ", "Admin sẽ trả lời bạn sớm nhất."}
    };

    public string GetReply(string message)
    {
        message = message.ToLower();
        foreach (var key in _responses.Keys)
        {
            if (message.Contains(key))
                return _responses[key];
        }
        return null;
    }
}
