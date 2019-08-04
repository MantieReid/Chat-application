using SausageChat.Networking;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SausageChat.Core;
using SausageChat.Core.Networking;
using System.Windows.Input;
using System.Threading;

namespace SausageChat
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
            InitializeComponent();
            var vm = new ViewModel();
            DataContext = vm;
            SausageServer.Vm = vm;
            SausageServer.Mw = this;
        }

        /// <summary>
        /// Adds a message to the chat box
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="SelectionChangedEventArgs"/></param>
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) //event for user list box
        {
            try
            {
                SausageServer.Vm.SelectedUser = (SausageConnection)e.AddedItems[0];
            }
            catch (Exception ex)
            { }
        }

        /// <summary>
        /// The Mute_Button_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private async void Mute_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await SausageServer.Mute(SausageServer.Vm.SelectedUser);
            }
            catch(Exception ex)
            {
                AddTextToDebugBox(ex.ToString());
            }
        }

        /// <summary>
        /// The Kick_Button_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private async void Kick_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await SausageServer.Kick(SausageServer.Vm.SelectedUser);
            }
            catch (Exception ex)
            {
                AddTextToDebugBox(ex.ToString());
            }
        }

        /// <summary>
        /// The Ban_Button_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private async void Ban_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await SausageServer.Ban(SausageServer.Vm.SelectedUser);
            }
            catch (Exception ex)
            {
                AddTextToDebugBox(ex.ToString());
            }
        }

        /// <summary>
        /// The AddTextToDebugBox
        /// </summary>
        /// <param name="text">The text<see cref="string"/></param>
        public void AddTextToDebugBox(string text)
        {
            DebugBox.Text += text;
        }

        /// <summary>
        /// The Info_Button_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void Info_Button_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// The Button_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// The Stop_Server_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void Stop_Server_Click(object sender, RoutedEventArgs e)
        {
            SausageServer.Close();
        }

        /// <summary>
        /// The Start_server_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void Start_server_Click(object sender, RoutedEventArgs e)
        {
            SausageServer.Open();
        }

        /// <summary>
        /// The Server_message_button_TextChanged
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="TextChangedEventArgs"/></param>
        private void Server_message_button_TextChanged(object sender, TextChangedEventArgs e)
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
        /// The Send_message_Button_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void Send_message_Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Messages_KeyDown(object sender, KeyEventArgs e)
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
    }
}