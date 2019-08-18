using System.Collections.ObjectModel;
using SausageChat.Networking;
using SausageChat.Core.Messaging;
using SausageChat.Core;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using System.Windows;

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
            var dictionary = new ResourceDictionary();
            dictionary.Source = new Uri("pack://application:,,,/MaterialDesignThemes.MahApps;component/Themes/MaterialDesignTheme.MahApps.Dialogs.xaml");

            Messages = new ObservableCollection<IMessage>();
            ConnectedUsers = new ObservableCollection<SausageConnection>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}