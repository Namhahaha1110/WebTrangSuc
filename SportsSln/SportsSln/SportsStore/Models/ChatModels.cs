using System;

namespace SportsStore.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Sender { get; set; }       // tên khách hoặc "Admin"
        public string Receiver { get; set; }     // "Admin" hoặc khách username
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
    }

    public class ChatUser
    {
        public string UserName { get; set; }
        public string ConnectionId { get; set; }
        public bool IsOnline { get; set; }
    }
}
