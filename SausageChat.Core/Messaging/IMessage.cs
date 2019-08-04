namespace SausageChat.Core.Messaging
{
    public interface IMessage
    {
        string Content { get; set; }
        string ToString();
    }
}
