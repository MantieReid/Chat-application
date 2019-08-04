using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using SausageChat.Core;

namespace SausageChat.Core
{
    /// <summary>
    /// a special implementation of an observable list that acts like a dictionary (and notifies PropertyChanged)
    /// </summary>
    public class SausageUserList : INotifyCollectionChanged
    {
        public ObservableCollection<User> Users { get; set; }
        public static SynchronizationContext UiCtx { get; set; }

        public User this[Guid guid]
        {
            get
            {
                return Users.First(x => x.Guid == guid);
            }
            set
            {
                var index = Users.IndexOf(Users.First(x => x.Guid == guid));

                UiCtx.Send(x => Users[index] = value);
                NotifyCollectionChanged(NotifyCollectionChangedAction.Replace);
            }
        }

        public SausageUserList() => Users = new ObservableCollection<User>();
        public SausageUserList(User[] users) => Users = new ObservableCollection<User>(users);
        
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void NotifyCollectionChanged(NotifyCollectionChangedAction CCA, [CallerMemberName] string CollectionName = "")
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(CCA));
        }

        public void Remove(Guid guid)
        {
            UiCtx.Send(y => Users.Remove(Users.FirstOrDefault(x => x.Guid == guid)));
            UiCtx.Send(x => NotifyCollectionChanged(NotifyCollectionChangedAction.Remove));
        }

        public void Remove(User user)
        {
            UiCtx.Send(x => Users.Remove(user));

            UiCtx.Send(x => NotifyCollectionChanged(NotifyCollectionChangedAction.Remove));
        }

        public void Add(User user)
        {
            UiCtx.Send(x => Users.Add(user));
            UiCtx.Send(x => NotifyCollectionChanged(NotifyCollectionChangedAction.Add));
        }

        public void Add(Guid guid)
        {
            Add(new User(guid: guid));
        }
    }
}
