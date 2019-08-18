using MahApps.Metro.Controls;
using SausageChat.Core;
using SausageChat.Core.Networking;
using SausageChatClient.Networking;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SausageChatClient
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            SausageUserList.UiCtx = SynchronizationContext.Current;
            ViewModel Vm = new ViewModel();
            SausageClient.Mw = this;
            SausageClient.Vm = Vm;
            this.DataContext = Vm;
        }

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Always test MultiValueConverter inputs for non-null
            // (to avoid crash bugs for views in the designer)
            if (values[0] is bool && values[1] is bool)
            {
                bool hasText = !(bool)values[0];
                bool hasFocus = (bool)values[1];

                if (hasFocus || hasText)
                    return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public string Which_IP_is_selected()
        {
            string option = "";
            if (Select_IP_Combo_box.SelectedIndex == 0)
                option = "Disco";
            else if (Select_IP_Combo_box.SelectedIndex == 1)
                option = "Monte";

            return option;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void Send_message_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(User_Message_client_Copy.Text))
            {
                PacketFormat packet = new PacketFormat(PacketOption.ClientMessage)
                {
                    Guid = SausageClient.ClientInfo.Guid,
                    Content = User_Message_client_Copy.Text
                };
                SausageClient.Send(packet);

                User_Message_client_Copy.Text = string.Empty;
            }
        }

        private void User_Message_client_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void ListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
        }

        private void Select_IP_Combo_box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void Button_Click_change_name(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!SausageClient.Socket.Connected)
                    return;
            }
            catch (NullReferenceException)
            {
                return;
            }
            Window1 win2 = new Window1();
            win2.ShowDialog();

            if (Window1.UserInput != SausageClient.ClientInfo.Name)
                SausageClient.Rename(Window1.UserInput);
        }

        private void Connect_Button_Client_Click(object sender, RoutedEventArgs e)
        {
            SausageClient.Start(Which_IP_is_selected());
        }

        private void User_Message_client_Copy_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // send the message
                PacketFormat packet = new PacketFormat(PacketOption.ClientMessage)
                {
                    Guid = SausageClient.ClientInfo.Guid,
                    Content = User_Message_client_Copy.Text
                };
                SausageClient.Send(packet);
                // reset the text box
                User_Message_client_Copy.Text = string.Empty;
            }
        }

        private void Disconnect_Button_Client_Click(object sender, RoutedEventArgs e) => SausageClient.Disconnect();

        private void ContextMenuBanUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SausageClient.Ban(SausageClient.Vm.SelectedUser.Guid);
            }
            catch (Exception) { }
        }

        private void ContextMenuKickUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SausageClient.Kick(SausageClient.Vm.SelectedUser.Guid);
            }
            catch (Exception) { }
        }

        private void ContextMenuMuteUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SausageClient.Mute(SausageClient.Vm.SelectedUser.Guid);
            }
            catch (Exception) { }
        }

        private void ContextMenuAddFriend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SausageClient.AddFriend(SausageClient.Vm.SelectedUser.Guid);
            }
            catch (Exception) { }
        }
    }
}