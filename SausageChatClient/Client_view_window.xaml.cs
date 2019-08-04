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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
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

        /// <summary>
        /// The Which_IP_is_selected
        /// </summary>
        /// <returns>The <see cref="string"/></returns>
        public string Which_IP_is_selected()
        {
            string option = "";
            if (Select_IP_Combo_box.SelectedIndex == 0)
                option = "Disco";
            else if (Select_IP_Combo_box.SelectedIndex == 1)
                option = "Monte";

            return option;
        }

        /// <summary>
        /// The ListBox_SelectionChanged
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="SelectionChangedEventArgs"/></param>
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        /// <summary>
        /// The Chat_Box_TextChanged
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="TextChangedEventArgs"/></param>
        private void Chat_Box_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        /// <summary>
        /// The Send_message_Button_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
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
            }
        }

        /// <summary>
        /// The User_Message_client_TextChanged
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="TextChangedEventArgs"/></param>
        private void User_Message_client_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        /// <summary>
        /// The ListBoxItem_Selected
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void ListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// The Select_IP_Combo_box_SelectionChanged
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="SelectionChangedEventArgs"/></param>
        private void Select_IP_Combo_box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        /// <summary>
        /// The Button_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void Button_Click_change_name(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!SausageClient.Socket.Connected)
                    return;
            }
            catch(NullReferenceException)
            {
                return;
            }
            Window1 win2 = new Window1();
            win2.ShowDialog();

            if (Window1.UserInput != SausageClient.ClientInfo.Name)
                SausageClient.Rename(Window1.UserInput);
        }

        /// <summary>
        /// The Ban_Button_client_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void Ban_Button_client_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// The Mute_Button_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void Mute_Button_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// The Kick_Button_client_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void Kick_Button_client_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// The Info_button_client_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void Info_button_client_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// The Freinds_button_client_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void Freinds_button_client_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// The Connect_Button_Client_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void Connect_Button_Client_Click(object sender, RoutedEventArgs e)
        {
            SausageClient.Start(Which_IP_is_selected());
        }

        /// <summary>
        /// The Add_Freind_button_client_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void Add_Freind_button_client_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// The User_Message_client_Copy_KeyDown
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="KeyEventArgs"/></param>
        private void User_Message_client_Copy_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // send the message
                SausageClient.Send(User_Message_client_Copy.Text);
                // reset the text box
                User_Message_client_Copy.Text = string.Empty;
            }
        }

        private void Disconnect_Button_Client_Click(object sender, RoutedEventArgs e) => SausageClient.Disconnect();

        private void ContextMenuBanUser_Click(object sender, RoutedEventArgs e) => SausageClient.Ban(SausageClient.Vm.SelectedUser.Guid);

        private void ContextMenuKickUser_Click(object sender, RoutedEventArgs e) => SausageClient.Kick(SausageClient.Vm.SelectedUser.Guid);

        private void ContextMenuMuteUser_Click(object sender, RoutedEventArgs e) => SausageClient.Mute(SausageClient.Vm.SelectedUser.Guid);

        private void ContextMenuAddFriend_Click(object sender, RoutedEventArgs e) => SausageClient.AddFriend(SausageClient.Vm.SelectedUser.Guid);

        private void SendMessageToFriendMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveFriendMenuitem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}