using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wpf_hands_on
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            txtSelection.Text = "Selected text is '";
            txtSelection.Text += textBox.SelectedText + "' with length of ";
            txtSelection.Text += textBox.SelectionLength;
        }

        private void submitBtn_Click(object sender, RoutedEventArgs e)
        {
            submitBtn.IsEnabled = false;
            submitBtn.Content = "Submitted";
        }

        private void cbAllStack_Checked(object sender, RoutedEventArgs e)
        {
            bool cbAllStackStatus = (cbAllStack.IsChecked == true);
            cStack.IsChecked = cbAllStackStatus;
            pythonStack.IsChecked = cbAllStackStatus;
            labviewStack.IsChecked = cbAllStackStatus;
        }

        private void cbStack_Changed(object sender, RoutedEventArgs e)
        {
            if (cStack.IsChecked == true && pythonStack.IsChecked == true && labviewStack.IsChecked == true)
                cbAllStack.IsChecked = true;
            else if (cStack.IsChecked == false && pythonStack.IsChecked == false && labviewStack.IsChecked == false)
                cbAllStack.IsChecked = false;
            else
                cbAllStack.IsChecked = null;
        }
    }
}