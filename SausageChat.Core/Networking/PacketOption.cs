namespace SausageChat.Core.Networking
{
    public enum PacketOption
    {
        // General server packet (Server messages)
        IsServer,
        // any client message (includes GUID)
        ClientMessage,
        // when a user joins, he will get a list of all users (from the server, this will only be sent to him)
        UserList,
        // on user banned (includes GUID)
        UserBanned,
        // on user kicked (includes GUID)
        UserKicked,
        // on user muted (includes GUID)
        UserMuted,
        // on user unmuted (includes GUID)
        UserUnmuted,
        // on user disconnect (includes GUID)
        UserDisconnected,
        // on user connect will be sent to all other clients with his GUID & name
        UserConnected,
        // on user change name (includes GUID & NewName)
        NameChange,
        // when a client joins the server, he needs to get a GUID
        GetGuid,
        // Friend request sent
        FriendRequest,
        // Friend request accepted
        FriendRequestAccepted,
        // Friend request denied
        FriendRequestDenied
    }
}
