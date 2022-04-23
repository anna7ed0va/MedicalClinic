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

namespace MedicalClinic
{
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class MessageBox : Window
    {
        public MessageBox()
        {
            InitializeComponent();        
        }


        MessageBoxResult Result = MessageBoxResult.None;

        public static MessageBoxResult Show(string message)
        {
            var dialog = new MessageBox();
            dialog.MessageContainer.Text = message;
            dialog.FontSize = 16;
            dialog.ShowDialog();
            return dialog.Result;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
