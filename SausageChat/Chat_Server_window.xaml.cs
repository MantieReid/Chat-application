using SausageChat.Core;
using SausageChat.Core.Networking;
using SausageChat.Networking;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SausageChat
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            SausageUserList.UiCtx = SynchronizationContext.Current;
            InitializeComponent();
            var vm = new ViewModel();
            DataContext = vm;
            SausageServer.Vm = vm;
            SausageServer.Mw = this;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) //event for user list box
        {
            try
            {
                SausageServer.Vm.SelectedUser = (SausageConnection)e.AddedItems[0];
            }
            catch (Exception ex)
            { }
        }

        private async void Mute_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await SausageServer.Mute(SausageServer.Vm.SelectedUser);
            }
            catch (Exception ex)
            {
                //AddTextToDebugBox(ex.ToString());
            }
        }

        private async void Kick_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await SausageServer.Kick(SausageServer.Vm.SelectedUser);
            }
            catch (Exception ex)
            {
                //AddTextToDebugBox(ex.ToString());
            }
        }

        private async void Ban_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await SausageServer.Ban(SausageServer.Vm.SelectedUser);
            }
            catch (Exception ex)
            {
                //AddTextToDebugBox(ex.ToString());
            }
        }

        public void AddTextToDebugBox(string text)
        {
        }

        private void Stop_Server_Click(object sender, RoutedEventArgs e)
        {
            SausageServer.Close();
        }

        private void Start_server_Click(object sender, RoutedEventArgs e)
        {
            SausageServer.Open();
        }

        private void Server_message_button_TextChanged(object sender, TextChangedEventArgs e)
        {
        }



        private async void Server_message_input_box_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // send the message
                await SausageServer.Log(new PacketFormat(PacketOption.IsServer)
                {
                    Content = Server_message_input_box.Text
                });
                // reset the text box
                Server_message_input_box.Text = "";
            }
        }

        private async void ContextMenuMute_Click(object sender, RoutedEventArgs e)
        {
            await SausageServer.Mute(SausageServer.Vm.SelectedUser);
        }

        private async void ContextMenuKick_Click(object sender, RoutedEventArgs e)
        {
            await SausageServer.Kick(SausageServer.Vm.SelectedUser);
        }

        private async void ContextMenuBan_Click(object sender, RoutedEventArgs e)
        {
            await SausageServer.Ban(SausageServer.Vm.SelectedUser);
        }

        private void Send_message_Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ContextMenuAdmin_Click(object sender, RoutedEventArgs e)
        {
            PacketFormat packet = new PacketFormat(PacketOption.AdminPermsRecieved)
            {
                Guid = SausageServer.Vm.SelectedUser.UserInfo.Guid
            };
            SausageServer.Log(packet);
        }
    }
}
