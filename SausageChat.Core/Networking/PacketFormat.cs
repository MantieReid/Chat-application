using System;
using SausageChat.Core;
using System.Collections.Generic;

namespace SausageChat.Core.Networking
{
    public class PacketFormat
    {
        public PacketOption Option { get; set; }
        public Guid Guid { get; set; }
        public Guid Sender { get; set; }
        public string NewName { get; set; }
        public User[] UsersList { get; set; }
        public string Content { get; set; } = "";

        public PacketFormat(PacketOption packetOption) => Option = packetOption;

        // for Json.NET
        #region JsonMethods

        public bool ShouldSerializeGuid() => Option != PacketOption.IsServer ||
                                             Option != PacketOption.UserList;
        public bool ShouldSerializeNewName() => Option == PacketOption.NameChange ||
                                                Option == PacketOption.UserConnected;
        public bool ShouldSerializeUsersList() => Option == PacketOption.UserList;
        public bool ShouldSerializeContent() => Option != PacketOption.NameChange ||
                                              Option != PacketOption.UserList ||
                                              Option != PacketOption.UserConnected ||
                                              Option != PacketOption.UserDisconnected;
        public bool ShouldSerializeSender() => Option == PacketOption.UserBanned ||
                                              Option == PacketOption.UserKicked ||
                                              Option == PacketOption.UserMuted ||
                                              Option == PacketOption.UserUnmuted ||
                                              Option == PacketOption.FriendRequest;

        #endregion
    }
}
