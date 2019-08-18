using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Net;
using System.Net.Sockets;
using SausageChat.Core.Messaging;
using SausageChat.Core;
using SausageChat.Core.Networking;
using System.Linq;
using Newtonsoft.Json;
using System.Threading;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace SausageChatClient.Networking
{
    static class SausageClient
    {
        // not a prop to pass it as ref in StripData()
        public static byte[] Data = new byte[1024];
        public static Dictionary<string, IPEndPoint> IpPool { get; set; } = new Dictionary<string, IPEndPoint>()
        {
            ["Disco"] = new IPEndPoint(IPAddress.Parse("93.172.118.193"), 60000)
        };
        public static Dictionary<string, ObservableCollection<User>> Friends
        {
            get
            {
                return Vm.Friends;
            }
            set
            {
                Vm.Friends = value;
            }
        }
        public static IPEndPoint ServerIp { get; set; }
        public static Socket Socket { get; set; }
        public static MainWindow Mw { get; set; }
        public static ViewModel Vm { get; set; }
        public static User ClientInfo { get; set; }
        public static SausageUserList UsersList {
            get
            {
                return Vm.UsersList;
            }
            set
            {
                Vm.UsersList = value;
            }
        }
        public static SynchronizationContext UiCtx { get; set; }

        public static bool Contains(this Dictionary<string, ObservableCollection<User>> friends, Guid guid) =>
            friends["OnlineFriends"].Any(x => x.Guid == guid) || friends["OfflineFriends"].Any(x => x.Guid == guid);

        public static async Task Start(string option)
        {
            try
            {
                if (Socket.Connected) return;
            }
            // socket will be default null at the beggining
            catch (NullReferenceException) { }
            try
            {
                ServerIp = IpPool[option];
                Socket = new Socket(ServerIp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                await Socket.ConnectAsync(ServerIp);
                UiCtx = SynchronizationContext.Current;
                Log(new ServerMessage("Connected"));
                Listen();
            }
            catch (SocketException)
            {
                Socket.Close();
            }
        }

        public static void Stop() => Disconnect();

        public static void Listen()
        {
            if (!Socket.Connected) return;

            try
            {
                Socket.BeginReceive(Data, 0, Data.Length, SocketFlags.None, OnMessageRecieved, null);
            }
            catch(SocketException)
            {
                Disconnect();
            }
        }

        public static void OnMessageRecieved(IAsyncResult ar)
        {
            try
            {
                Socket.EndReceive(ar); 
            }
            catch(SocketException)
            {
                return;
            }

            SausageHelper.StripData(ref Data);
            Parse(Encoding.ASCII.GetString(Data));

            Data = new byte[1024];
            Listen();
        }

        private static void Parse(string msg)
        {
            PacketFormat Message = JsonConvert.DeserializeObject<PacketFormat>(msg);

            switch (Message.Option)
            {
                case PacketOption.ClientMessage:
                    Log(new UserMessage(Message.Content, UsersList[Message.Guid]));
                    break;
                case PacketOption.IsServer:
                    Log(new ServerMessage(Message.Content));
                    break;
                case PacketOption.NameChange:
                    if (Message.Guid != ClientInfo.Guid)
                    {
                        Log(new ServerMessage($"{UsersList[Message.Guid].Name} has changed their name to {Message.NewName}"));
                        UsersList[Message.Guid].Name = Message.NewName;
                        break;
                    }
                    Log(new ServerMessage($"You changed your name to {ClientInfo.Name}"));
                    break;
                case PacketOption.UserBanned:
                    if (ClientInfo.Guid == Message.Guid)
                    {
                        Disconnect();
                        Log(new ServerMessage("You've been banned:"));
                        Log(new ServerMessage(Message.Content));
                        break;
                    }
                    Log(new ServerMessage($"{UsersList[Message.Guid]} has been banned:"));
                    Log(new ServerMessage(Message.Content));
                    UsersList.Remove(Message.Guid);
                    break;
                case PacketOption.UserKicked:
                    if (ClientInfo.Guid == Message.Guid)
                    {
                        Disconnect();
                        Log(new ServerMessage("You've been kicked:"));
                        Log(new ServerMessage(Message.Content));
                        break;
                    }
                    Log(new ServerMessage($"{UsersList[Message.Guid]} has been kicked:"));
                    Log(new ServerMessage(Message.Content));
                    UsersList.Remove(Message.Guid);
                    break;
                case PacketOption.UserMuted:
                    if (ClientInfo.Guid == Message.Guid)
                    {
                        // disable chat box
                        Log(new ServerMessage("You've been muted:"));
                    }
                    else
                        Log(new ServerMessage($"{UsersList[Message.Guid]} has been muted:"));
                    Log(new ServerMessage(Message.Content));
                    UsersList[Message.Guid].IsMuted = true;
                    break;
                case PacketOption.UserUnmuted:
                    Log(new ServerMessage($"{UsersList[Message.Guid]} has been unmuted"));
                    UsersList[Message.Guid].IsMuted = false;
                    break;
                case PacketOption.UserConnected:
                    Log(new ServerMessage($"{Message.Guid} has joined"));
                    UsersList.Add(new User(Message.NewName, Message.Guid));
                    break;
                case PacketOption.UserDisconnected:
                    Log(new ServerMessage($"{UsersList[Message.Guid]} has disconnected"));
                    UsersList.Remove(Message.Guid);
                    break;
                case PacketOption.GetGuid:
                    ClientInfo = new User(Message.Guid.ToString(), Message.Guid);
                    Log(new ServerMessage($"{Message.Guid.ToString()} has joined."));
                    if(Message.UsersList != null)
                        UsersList.Add(Message.UsersList);
                    UsersList.Add(ClientInfo);
                    break;
                case PacketOption.FriendRequest:
                    MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                    string content = $"Friend request recieved from {UsersList[Message.Sender].Name}, Accept?";
                    string caption = "Sausage Chat friend request";
                    DialogResult result;

                    result = MessageBox.Show(content, caption, buttons);
                    if (result == DialogResult.Yes)
                    {
                        Log(new ServerMessage("Accepted friend request"));
                        Friends["OnlineFriends"].Add(UsersList[Message.Sender]);
                    }
                    else
                        break;
                    PacketFormat packet = new PacketFormat(PacketOption.FriendRequestAccepted)
                    {
                        Sender = ClientInfo.Guid,
                        Guid = Message.Sender
                    };
                    Send(packet);
                    break;
                case PacketOption.FriendRequestAccepted:
                    Friends["OnlineFriends"].Add(UsersList[Message.Guid]);
                    Log(new ServerMessage($"Friend request accepted by {UsersList[Message.Guid].Name}"));
                    break;
                case PacketOption.FriendRequestDenied:
                    Log(new ServerMessage("Friend request denied"));
                    break;
                case PacketOption.AdminPermsRecieved:
                    if (Message.Guid == ClientInfo.Guid)
                    {
                        ClientInfo.IsAdmin = true;
                        Log(new ServerMessage($"You are now admin"));
                    }
                    else
                        Log(new ServerMessage($"{UsersList[Message.Guid]} is now admin"));
                    break;
                case PacketOption.AdminPermsRemoved:
                    if (Message.Guid == ClientInfo.Guid)
                    {
                        ClientInfo.IsAdmin = false;
                        Log(new ServerMessage($"You are no longer admin"));
                    }
                    else
                        Log(new ServerMessage($"{UsersList[Message.Guid]} is no longer admin"));
                    break;
            }
        }

        public static void Send(string message)
        {
            if (!Socket.Connected) return;

            byte[] bytesMessage = Encoding.ASCII.GetBytes(message);
            

            try
            {
                Socket.BeginSend(bytesMessage, 0, bytesMessage.Length, SocketFlags.None, OnSendComplete, null);
            }
            catch (SocketException)
            {
                return;
            }
        }

        public static void Send(PacketFormat packet) => Send(JsonConvert.SerializeObject(packet));

        private static void OnSendComplete(IAsyncResult ar)
        {
            try
            {
                Socket.EndSend(ar);
            }
            catch(SocketException)
            {
                return;
            }
        }

        public static void Log(string msg) => Log(new UserMessage(msg));

        public static void Log(IMessage message)
        {
            UiCtx.Send(x => Vm.Messages.Add(message));
        }

        public static void Ban(Guid guid, string Reason = null) => BanKickMuteWrapper(guid, PacketOption.UserBanned, Reason);

        public static void Kick(Guid guid, string Reason = null) => BanKickMuteWrapper(guid, PacketOption.UserKicked, Reason);

        public static void Mute(Guid guid, string Reason = null) => BanKickMuteWrapper(guid, PacketOption.UserMuted, Reason);

        public static void Unmute(Guid guid) => BanKickMuteWrapper(guid, PacketOption.UserUnmuted);

        private static void BanKickMuteWrapper(Guid guid, PacketOption option, string Reason = null)
        {
            if (!ClientInfo.IsAdmin) return;
            PacketFormat packet;
            if (option != PacketOption.UserUnmuted)
            {
                if (Reason == null) Reason = "Place Holder Reason";
                packet = new PacketFormat(option)
                {
                    Sender = ClientInfo.Guid,
                    Guid = guid,
                    Content = Reason
                };
            }
            else
            {
                packet = new PacketFormat(PacketOption.UserUnmuted)
                {
                    Sender = ClientInfo.Guid,
                    Guid = guid
                };
            }
            Send(packet);
        }

        // the server will return the rename message thus no need for logging (in client)
        public static void Rename(string newName)
        {
            UsersList[ClientInfo.Guid].Name = newName;
            PacketFormat packet = new PacketFormat(PacketOption.NameChange)
            {
                Guid = ClientInfo.Guid,
                NewName = newName
            };
            Send(packet);
        }

        // Needs implementation
        public static void AddFriend(Guid guid)
        {
            PacketFormat packet = new PacketFormat(PacketOption.FriendRequest)
            {
                Sender = ClientInfo.Guid,
                Guid = guid,
            };

            Send(packet);
            Log(new ServerMessage("Friend request sent"));
        }

        public static void Disconnect(PacketOption? Po = null)
        {
            try
            {
                if (Socket.Connected) return;

                Socket.Close();
                UsersList = new SausageUserList();

                Log(new ServerMessage("Disconnected"));
            }
            catch(SocketException)
            {
                throw new NotImplementedException();
            }
        }
    }
}
