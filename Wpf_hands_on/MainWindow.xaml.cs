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
        WikipediaDetails details = new WikipediaDetails() { Name = "Harith", Description = "Select some text from this description", IsFemale = true };
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = details;
        }

        private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            details.SelectedText = textBox.SelectedText;
            txtSelection.Text = "Selected text is '";
            txtSelection.Text += textBox.SelectedText + "' with length of ";
            txtSelection.Text += textBox.SelectionLength;
        }

        private void submitBtn_Click(object sender, RoutedEventArgs e)
        {
            submitBtn.IsEnabled = false;
            submitBtn.Content = "Submitted";
            MessageBox.Show(
                "Name : " + details.Name
                + "\nGender : " + (details.IsFemale ? "Female" : "Male")
                + "\nStacks : " + ((bool)cStack.IsChecked ? "C# " : "")
                                + ((bool)pythonStack.IsChecked ? "Python " : "")
                                + ((bool)labviewStack.IsChecked ? "Labview " : "")
                + "\nSelected Text : " + details.SelectedText
            );
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

        private void Male_Checked(object sender, RoutedEventArgs e)
        {
            details.IsFemale = false;
        }
        private void Female_Checked(object sender, RoutedEventArgs e)
        {
            details.IsFemale = true;
        }
    }

    public class WikipediaDetails
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsFemale { get; set; }
        public string SelectedText { get; set; }
    }
}