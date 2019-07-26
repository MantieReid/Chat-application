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
    // TD -> when renaming, make sure there isn't another user called the same name (would mess up Mute/Kick/Ban)
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
        public static Dictionary<Guid, User> UsersDictionary { get; set; } = new Dictionary<Guid, User>();
        public static List<IPAddress> Blacklisted { get; set; } = new List<IPAddress>();
        public static IPEndPoint LocalIp { get; set; } = new IPEndPoint(IPAddress.Any, PORT);
        public static SynchronizationContext UiCtx { get; set; }
        
        public static void Open()
        {
            if(!IsOpen)
            {
                MainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                ConnectedUsers = new ObservableCollection<SausageConnection>();
                MainSocket.Bind(LocalIp);
                MainSocket.Listen(10);
                UiCtx = SynchronizationContext.Current;
                MainSocket.BeginAccept(OnUserConnect, null);
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
                Vm.Messages.Add(new ServerMessage("Closed all sockets"));
            }
        }

        public static async Task Ban(SausageConnection user)
        {
            try
            {
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
                    UiCtx.Send(x => ConnectedUsers.Remove(user), null);
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
                // user exists
                if (ConnectedUsers.Any(x => x.UserInfo.Guid == user.UserInfo.Guid))
                {
                    PacketFormat packet = new PacketFormat(PacketOption.UserKicked)
                    {
                        Guid = user.UserInfo.Guid,
                        Content = "Place-holder reason"
                    };
                    await Log(packet);
                    user.Disconnect();
                    UiCtx.Send(x => Vm.ConnectedUsers.Remove(user), null);
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
                    Log(packet);
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
                    Log(packet);
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

        public static void OnUserConnect(IAsyncResult ar)
        {
            var user = new SausageConnection(MainSocket.EndAccept(ar));
            if (!Blacklisted.Any(x => x == user.Ip.Address))
            {
                UiCtx.Send(x => ConnectedUsers.Add(user), null);
                UiCtx.Send(x => Vm.ConnectedUsers = SortUsersList(), null);
                UiCtx.Send(x => Mw.AddTextToDebugBox($"User connected on {user.Ip}\n"), null);
                UsersDictionary.Add(user.UserInfo.Guid, user.UserInfo);
                // global packet for all the users to know the user has joined
                PacketFormat GlobalPacket = new PacketFormat(PacketOption.UserConnected)
                {
                    Guid = user.UserInfo.Guid,
                    NewName = user.UserInfo.Name
                };
                // local packet for the user (who joined) to get his GUID
                PacketFormat LocalPacket = new PacketFormat(PacketOption.GetGuid)
                {
                    Guid = user.UserInfo.Guid
                };
                user.SendAsync(LocalPacket);
                Log(GlobalPacket, user);
                UiCtx.Send(x => Mw.AddTextToDebugBox($"Logging now..."), null);
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
                UiCtx.Send(x => Vm.Messages.Add(new UserMessage(message.Content, UsersDictionary[message.Guid])), null);
            else
                UiCtx.Send(x => Vm.Messages.Add(new ServerMessage(message.Content)), null);

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
