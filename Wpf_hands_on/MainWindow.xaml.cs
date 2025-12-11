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
        WikipediaDetails details = new WikipediaDetails() { Name = "Harith", Description = "Select some text from this description", Gender = "male" };
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = details;
            projectTxt.ItemsSource = new List<string>() { "abc", "def", "ghi" };

            List<User> users = new List<User>();
            users.Add(new User() { Name = "vel", Stack = "C#" });
            users.Add(new User() { Name = "ak", Stack = "Python" });
            users.Add(new User() { Name = "sans", Stack = "C#" });
            TeammatesList.ItemsSource = users;
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
                + "\nGender : " + details.Gender
                + "\nStacks : " + string.Join(",", details.Stacks)
                + "\nSelected Text : " + details.SelectedText
                + "\nProject : " + details.Project
                + "\nTeammates : \n\t" + string.Join("\n\t",details.Teammates)
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
            CheckBox cb = sender as CheckBox;
            if ((bool)cb.IsChecked)
            {
                details.Stacks.Add((string)cb.Content);
            }
            else
            {
                details.Stacks.Remove((string)cb.Content);
            }
            if (cStack.IsChecked == true && pythonStack.IsChecked == true && labviewStack.IsChecked == true)
                cbAllStack.IsChecked = true;
            else if (cStack.IsChecked == false && pythonStack.IsChecked == false && labviewStack.IsChecked == false)
                cbAllStack.IsChecked = false;
            else
                cbAllStack.IsChecked = null;
        }

        private void Male_Checked(object sender, RoutedEventArgs e)
        {
            details.Gender = "male";
        }
        private void Female_Checked(object sender, RoutedEventArgs e)
        {
            details.Gender = "female";
        }

        private void projectTxt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            details.Project = (string)projectTxt.SelectedItem;
        }

        private void TeammatesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<User> Teammates = new List<User>();
            foreach (User teammate in TeammatesList.SelectedItems)
            {
                Teammates.Add(teammate);
            }
            details.Teammates = Teammates;
        }
    }

    public class GenderToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch (value.ToString().ToLower())
            {
                case "female":
                    return true;
                case "male":
                    return false;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                if ((bool)value == true)
                    return "female";
                else
                    return "male";
            }
            return "male";
        }
    }
    public class WikipediaDetails
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string SelectedText { get; set; } = "NA";
        public string Gender { get; set; }
        public List<string> Stacks { get; set; } = [];
        public string Project { get; set; } = "NA";
        public List<User> Teammates { get; set; }
    }

    public class User
    {
        public string Name { get; set; }
        public string Stack { get; set; }
        public override string ToString()
        {
            return $"{Name} - {Stack}";
        }
    }
}