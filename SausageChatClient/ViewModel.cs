using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SausageChat.Core.Messaging;
using System.ComponentModel;
using SausageChat.Core;
using System.Runtime.CompilerServices;

namespace SausageChatClient
{
    public class ViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<IMessage> messages;
        public ObservableCollection<IMessage> Messages
        {
            get
            {
                return messages;
            }
            set
            {
                messages = value;
                NotifyPropertyChanged();
            }
        }

        public SausageUserList UsersList { get; set; }

        public User SelectedUser { get; set; } = null;
        public User SelectedFriend { get; set; } = null;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Bind to: "OnlineFriends", "OfflineFriends"
        public Dictionary<string, ObservableCollection<User>> Friends { get; set; }

        public ViewModel()
        {
            UsersList = new SausageUserList();


            Friends = new Dictionary<string, ObservableCollection<User>>();
            Messages = new ObservableCollection<IMessage>();

        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}