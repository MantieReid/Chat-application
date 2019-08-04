using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using SausageChat.Core;
using SausageChat.Core.Messaging;
using SausageChat.Core.Networking;
using Newtonsoft.Json;

namespace SausageChat.Networking
{
    // TODO: Log everything in Json format
    class SausageConnection
    {
        public byte[] Data { get; set; } = new byte[1024];
        public IPEndPoint Ip { get; set; }
        public Socket Socket { get; set; }
        public User UserInfo { get; set; }

        public List<UserMessage> Messages { get; set; }

        public SausageConnection(Socket socket)
        {
            Socket = socket;
            Ip = Socket.RemoteEndPoint as IPEndPoint;
            UserInfo = new User();
            UserInfo.Name = UserInfo.Guid.ToString();

            Messages = new List<UserMessage>();

            ListenAsync();
        }

        /// <summary>
        /// TEST CTOR DON'T USE
        /// </summary>
        /// <param name="n"></param>
        public SausageConnection(string n) => UserInfo = new User(n);

        // needs change to PacketFormat
        public void SendAsync(string data) => SendAsync(Encoding.ASCII.GetBytes(data));

        public void SendAsync(PacketFormat packet) => SendAsync(JsonConvert.SerializeObject(packet));

        public void SendAsync(byte[] data)
        {
            try
            {
                Socket.BeginSend(data, 0, data.Length, SocketFlags.None, OnSendComplete, null);
            }
            catch (SocketException)
            {
                Disconnect();
            }
            catch(ObjectDisposedException)
            {
                Disconnect();
            }
        }

        private void OnSendComplete(IAsyncResult ar)
        {
            try
            {
                Socket.EndSend(ar);
            }
            catch(SocketException)
            {
                Disconnect();
            }
        }

        public void ListenAsync()
        {
            Socket.BeginReceive(Data, 0, Data.Length, SocketFlags.None, OnDataRecieve, null);
        }

        private void OnDataRecieve(IAsyncResult ar)
        {
            try
            {
                var BytesRec = Socket.EndReceive(ar);
                Parse(Encoding.ASCII.GetString(Data));
                ListenAsync();
            }
            catch(SocketException)
            {
                Disconnect();
            }
            catch(ObjectDisposedException)
            {
                Disconnect();
            }
        }

        // need to change the VM names lists, maybe change it to a dictionary
        public string Rename(string newName)
        {
            string oldName = UserInfo.Name;
            UserInfo.Name = newName;
            SausageServer.Vm.ConnectedUsers.First(x => x == this).UserInfo.Name = newName;  
            SausageServer.Vm.ConnectedUsers = SausageServer.SortUsersList();
            return oldName;
        }

        public void Disconnect()
        {
            SausageServer.Vm.ConnectedUsers = SausageServer.SortUsersList();
            Socket.Close();
        }

        private void Parse(string msg)
        {
            PacketFormat Message = JsonConvert.DeserializeObject<PacketFormat>(msg);

            switch (Message.Option)
            {
                case PacketOption.ClientMessage:
                    if(!SausageServer.UsersDictionary[Message.Guid].IsMuted)
                        SausageServer.Log(Message);
                    break;
                case PacketOption.NameChange:
                    SausageServer.Log(Message);
                    SausageServer.UsersDictionary[Message.Guid].Name = Message.NewName;
                    break;
                // don't need to check for user connected as that is dealt with in OnUserConnect
                case PacketOption.UserDisconnected:
                    SausageServer.Log(Message);
                    Disconnect();
                    SausageServer.UsersDictionary.Remove(Message.Guid);
                    break;
                case PacketOption.UserList:
                    PacketFormat packet = new PacketFormat(PacketOption.UserList)
                    {
                        UsersList = SausageServer.UsersDictionary.Users.ToArray()
                    };
                    SendAsync(packet);
                    break;
                case PacketOption.UserBanned:
                    if(UserInfo.IsAdmin)
                        SausageServer.Ban(SausageServer.ConnectedUsers.First(x => x.UserInfo.Guid == Message.Guid));
                    break;
                case PacketOption.UserKicked:
                    if(UserInfo.IsAdmin)
                        SausageServer.Kick(SausageServer.ConnectedUsers.First(x => x.UserInfo.Guid == Message.Guid));
                    break;
                case PacketOption.UserMuted:
                    if (UserInfo.IsAdmin)
                        SausageServer.Mute(SausageServer.ConnectedUsers.First(x => x.UserInfo.Guid == Message.Guid));
                    break;
                case PacketOption.UserUnmuted:
                    if (UserInfo.IsAdmin)
                        SausageServer.Unmute(SausageServer.ConnectedUsers.First(x => x.UserInfo.Guid == Message.Guid));
                    break;
                case PacketOption.FriendRequest:
                    SausageConnection reciever = SausageServer.ConnectedUsers.FirstOrDefault(x => x.UserInfo.Guid == Message.Guid);
                    if(reciever == null)
                        SendAsync(new PacketFormat(PacketOption.IsServer) { Content = "User not found" });
                    else
                        reciever.SendAsync(Message);
                    break;
                case PacketOption.FriendRequestAccepted:
                    SausageConnection r = SausageServer.ConnectedUsers.FirstOrDefault(x => x.UserInfo.Guid == Message.Guid);
                    r.SendAsync(Message);
                    break;
                case PacketOption.FriendRequestDenied:
                    SausageConnection rec = SausageServer.ConnectedUsers.FirstOrDefault(x => x.UserInfo.Guid == Message.Guid);
                    rec.SendAsync(Message);
                    break;
            }
        }

        public override string ToString() => UserInfo.Name;

        public override bool Equals(object obj)
        {
            if(obj is SausageConnection)
            {
                SausageConnection u = obj as SausageConnection;
                return u.Ip == this.Ip;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            var hashCode = -333338465;
            hashCode = hashCode * -1521134295 + EqualityComparer<byte[]>.Default.GetHashCode(Data);
            hashCode = hashCode * -1521134295 + UserInfo.IsMuted.GetHashCode();
            hashCode = hashCode * -1521134295 + UserInfo.IsAdmin.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<IPEndPoint>.Default.GetHashCode(Ip);
            hashCode = hashCode * -1521134295 + EqualityComparer<Socket>.Default.GetHashCode(Socket);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(UserInfo.Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<UserMessage>>.Default.GetHashCode(Messages);
            return hashCode;
        }
    }
}
