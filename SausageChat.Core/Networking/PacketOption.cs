namespace SausageChat.Core.Networking
{
    public enum PacketOption
    {
        // General server packet (Server messages)
        IsServer,
        // any client message (includes GUID)
        ClientMessage,
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
        // when a client joins the server, he needs to get a GUID and a list of users
        GetGuid,
        // Friend request sent
        FriendRequest,
        // Friend request accepted
        FriendRequestAccepted,
        // Friend request denied
        FriendRequestDenied,
        // DmMessage
        DmMessage,
        // Dm start request (from user1 to user2)
        DmStartRequest,
        // Dm Accept (from user2 back to user1)
        DmAccepted,
        // Dm Denied (from user2 back to user1)
        DmDenied,
        // Give admin perms
        AdminPermsRecieved,
        // Remove admin perms
        AdminPermsRemoved
    }
}
