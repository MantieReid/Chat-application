using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SausageChatClient
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Window1"/> class.
        /// </summary>
        public Window1()
        {



            InitializeComponent();
        }

        /// <summary>
        /// Defines the user_input_string
        /// </summary>
        public static string UserInput;// will be used to store the name the user has set for himself.  global

        /// <summary>
        /// The Enter_name_input_box_TextChanged
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="TextChangedEventArgs"/></param>
        private void Enter_name_input_box_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        /// <summary>
        /// The Set_name_Button_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void Set_name_Button_Click(object sender, RoutedEventArgs e)
        {
            UserInput = Enter_name_input_box.Text;
            

            this.Close();
        }
    }
}
