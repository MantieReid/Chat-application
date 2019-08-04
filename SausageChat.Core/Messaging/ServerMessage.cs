namespace SausageChat.Core.Messaging
{
    public class ServerMessage : IMessage
    {
        public string Content { get; set; }

        public ServerMessage(string content) => Content = content;

        public override string ToString() => Content;
    }
}
