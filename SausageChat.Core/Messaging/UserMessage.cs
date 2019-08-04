namespace SausageChat.Core.Messaging
{
    public class UserMessage : IMessage
    {
        public string Content { get; set; }
        public User Author { get; set; }

        public UserMessage(string Content, User Author = null)
        {
            this.Content = Content;
            if (Author == null)
                this.Author = new User();
            else
                this.Author = Author;
        }

        public override string ToString()
        {
            return $"<{Author}>: {Content}";
        }
    }
}
