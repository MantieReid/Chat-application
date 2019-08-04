using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SausageChat.Core.Messaging;

namespace SausageChat.Core
{
    public class User : INotifyPropertyChanged
    {
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                NotifyPropChanged();
            }
        }
        public Guid Guid { get; set; }
        public bool IsAdmin { get; set; } = false;
        public bool IsMuted { get; set; } = false;
        public List<IMessage> Messages { get; set; }

        public User(string Name = null, Guid guid = default(Guid))
        {
            if (guid == default(Guid))
                Guid = Guid.NewGuid();
            else
                Guid = guid;
            if (Name == null)
                this.Name = Guid.ToString();
            else
                this.Name = Name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropChanged([CallerMemberName] string PropName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropName));

        public override string ToString() => Name;

        #region JsonMethods

        public bool ShouldSerializeIsAdmin() => false;
        public bool ShouldSerializeIsMuted() => false;
        public bool ShouldSerializeMessages() => false;

        #endregion
    }
}
