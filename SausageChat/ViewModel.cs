using System.Collections.ObjectModel;
using SausageChat.Networking;
using SausageChat.Core.Messaging;
using SausageChat.Core;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;

namespace SausageChat
{
  class ViewModel : INotifyPropertyChanged
  {
        public ObservableCollection<IMessage> Messages { get; set; } //a collection of messages
                                                                 // User names (for user list)
        private ObservableCollection<SausageConnection> _connectedUsers;
        public ObservableCollection<SausageConnection> ConnectedUsers
        {
            get
            {
                return _connectedUsers;
            }
            set
            {
                _connectedUsers = value;
                NotifyPropertyChanged();
            }
        }

        public SausageConnection SelectedUser { get; set; } = null;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ViewModel()
        {
            Messages = new ObservableCollection<IMessage>();
            Messages.Add(new ServerMessage("Hello!"));
      Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!")); Messages.Add(new ServerMessage("Hello!"));
      ConnectedUsers = new ObservableCollection<SausageConnection>();
            ConnectedUsers.Add(new SausageConnection("Sally"));
            ConnectedUsers.Add(new SausageConnection("Bob"));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
