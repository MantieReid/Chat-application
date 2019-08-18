using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using SausageChat.Core.Messaging;
using SausageChat.Helpers;
using SausageChat.Core;
using System.Collections.ObjectModel;
using SausageChat.Core.Networking;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Threading;

namespace SausageChat.Networking
{
    static class SausageServer
    {
        public static bool IsOpen { get; set; } = false;
        public static ViewModel Vm { get; set; }
        public static MainWindow Mw { get; set; }
        public static Socket MainSocket { get; set; }
        public const int PORT = 60000;
        public static ObservableCollection<SausageConnection> ConnectedUsers
        {
            get
            {
                return Vm.ConnectedUsers;
            }
            set
            {
                Vm.ConnectedUsers = value;
            }
    }
        public static SausageUserList UsersDictionary { get; set; } = new SausageUserList();
        public static List<IPAddress> Blacklisted { get; set; } = new List<IPAddress>();
        public static IPEndPoint LocalIp { get; set; } = new IPEndPoint(IPAddress.Any, PORT);
        public static SynchronizationContext UiCtx { get; set; }
        
        public static void Open()
        {
            if (!IsOpen)
            {
                MainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                ConnectedUsers = new ObservableCollection<SausageConnection>();
                UsersDictionary = new SausageUserList();
                MainSocket.Bind(LocalIp);
                MainSocket.Listen(10);
                UiCtx = SynchronizationContext.Current;
                MainSocket.BeginAccept(OnUserConnect, null);
                IsOpen = true;
                UiCtx.Send(x => Vm.Messages.Add(new ServerMessage("Opened server")));
            }
            else
            {
                UiCtx.Send(x => Vm.Messages.Add(new ServerMessage("Server already open")));
            }
        }

        public static void Close()
        {
            if(IsOpen)
            {
                MainSocket.Close();
                foreach(SausageConnection u in ConnectedUsers)
                {
                    u.Disconnect();
                }
                UiCtx.Send(x => Vm.Messages = new ObservableCollection<IMessage>());
                UiCtx.Send(x => Vm.Messages.Add(new ServerMessage("Closed all sockets")));
                IsOpen = false;
            }
            else
            {
                MessageBox.Show("Server is already closed", "Sausage Server");
            }
        }

        // TODO: logging
        public static async Task Ban(SausageConnection user)
        {
            try
            {
                if (!(user.Socket.Connected || MainSocket.Connected)) return;
                // user exists
                if (ConnectedUsers.Any(x => x.UserInfo.Guid == user.UserInfo.Guid))
                {
                    Blacklisted.Add(user.Ip.Address);
                    PacketFormat packet = new PacketFormat(PacketOption.UserBanned)
                    {
                        Guid = user.UserInfo.Guid,
                        Content = "Place-holder reason"
                    };
                    Log(packet);
                    await Task.Delay(1000);
                    // delay for waiting on the client to recieve a message
                    user.Disconnect();
                    UiCtx.Send(x => ConnectedUsers.Remove(user));
                }
                else
                {
                    MessageBox.Show("User not found", "Ban result");
                }
            }
            catch (ArgumentNullException e)
            {
                MessageBox.Show($"User returned null {e}", "Exception Caught");
            }
        }
        
        public static async Task Kick(SausageConnection user)
        {
            try
            {
                if (!(user.Socket.Connected || MainSocket.Connected)) return;
                // user exists
                if (ConnectedUsers.Any(x => x.UserInfo.Guid == user.UserInfo.Guid))
                {
                    PacketFormat packet = new PacketFormat(PacketOption.UserKicked)
                    {
                        Guid = user.UserInfo.Guid,
                        Content = "Place-holder reason"
                    };
                    await Log(packet);
                    // delay for waiting on the client to recieve a message
                    await Task.Delay(1000);
                    user.Disconnect();
                    UiCtx.Send(x => Vm.ConnectedUsers.Remove(user));
                }
                else
                {
                    MessageBox.Show("User not found", "Kick result");
                }
            }
            catch(ArgumentNullException e)
            {
                MessageBox.Show($"User returned null {e}", "Exception Caught");
            }
        }

        public static async Task Mute(SausageConnection user)
        {
            try
            {
                // user exists
                if (ConnectedUsers.Any(x => x.UserInfo.Guid == user.UserInfo.Guid))
                {
                    PacketFormat packet = new PacketFormat(PacketOption.UserMuted)
                    {
                        Guid = user.UserInfo.Guid,
                        Content = "Place-holder reason"
                    };
                    user.UserInfo.IsMuted = true;
                    await Log(packet);
                }
                else
                {
                    MessageBox.Show("User not found", "Kick result");
                }
            }
            catch(ArgumentNullException e)
            {
                MessageBox.Show($"User returned null {e}", "Exception Caught");
            }
        }

        public static async Task Unmute(SausageConnection user)
        {
            try
            {
                // user exists
                if (ConnectedUsers.Any(x => x.UserInfo.Guid == user.UserInfo.Guid))
                {
                    PacketFormat packet = new PacketFormat(PacketOption.UserUnmuted)
                    {
                        Guid = user.UserInfo.Guid
                    };
                    user.UserInfo.IsMuted = false;
                    await Log(packet);
                }
                else
                {
                    MessageBox.Show("User not found", "Kick result");
                }
            }
            catch (ArgumentNullException e)
            {
                MessageBox.Show($"User returned null {e}", "Exception Caught");
            }
        }

        // TODO: Add user list
        public static void OnUserConnect(IAsyncResult ar)
        {
            SausageConnection user;
            try
            {
                user = new SausageConnection(MainSocket.EndAccept(ar));
            }
            catch(SocketException ex)
            {
                Close();
                return;
            }
            catch(ObjectDisposedException ex)
            {
                return;
            }
            if (!Blacklisted.Any(x => x == user.Ip.Address))
            {
                UiCtx.Send(x => ConnectedUsers.Add(user));
                UiCtx.Send(x => Vm.ConnectedUsers = SortUsersList());
                UiCtx.Send(x => Vm.Messages.Add(new ServerMessage($"{user} has connected")));
                UiCtx.Send(x => Mw.AddTextToDebugBox($"User connected on {user.Ip}\n"));
                // global packet for all the users to know the user has joined
                PacketFormat GlobalPacket = new PacketFormat(PacketOption.UserConnected)
                {
                    Guid = user.UserInfo.Guid,
                    NewName = user.UserInfo.Name
                };
                // local packet for the user (who joined) to get his GUID
                PacketFormat LocalPacket = new PacketFormat(PacketOption.GetGuid)
                {
                    Guid = user.UserInfo.Guid,
                    UsersList = UsersDictionary.ToArray()
                };
                UsersDictionary.Add(user.UserInfo);
                user.SendAsync(LocalPacket);
                Log(GlobalPacket, user);
            }
            else
            {
                // doesn't log if the user is blacklisted
                user.Disconnect();
            }
            MainSocket.BeginAccept(OnUserConnect, null);
        }

        // TODO: make a switch for the user messge (some packets don't have content)
        public async static Task Log(PacketFormat message, SausageConnection ignore = null)
        {
            if (message.Option == PacketOption.ClientMessage)
                UiCtx.Send(x => Vm.Messages.Add(new UserMessage(message.Content, UsersDictionary[message.Guid])));
            else
                switch(message.Option)
                {
                    case PacketOption.NameChange:
                        UiCtx.Send(x => Vm.Messages.Add(
                            new ServerMessage($"{UsersDictionary[message.Guid]} changed their name to {message.NewName}")));
                        break;
                    default:
                        UiCtx.Send(x => Vm.Messages.Add(new ServerMessage(message.Content)));
                        break;
                }

            foreach(var user in ConnectedUsers)
            {
                if(user != ignore)
                    user.SendAsync(JsonConvert.SerializeObject(message));
            }
        }

        public static ObservableCollection<SausageConnection> SortUsersList()
        {
            List<SausageConnection> names = new List<SausageConnection>(Vm.ConnectedUsers);
            names.Sort(new ConnectionComparer());

            return new ObservableCollection<SausageConnection>(names);
        }
    }
}
