using System.Windows;
using System.Windows.Controls;
using StudentDataViewer.View;
using StudentDataViewer.ViewModel;

namespace StudentDataViewer.Views
{
    /// <summary>
    /// Interaction logic for LeftPanel.xaml
    /// </summary>
    public partial class LeftPanelUserControl : UserControl
    {
        private MainViewModel viewModel;
        private AddStudentView _addStudentView;

        /// <summary>
        /// Sets dataContext and viewModel.
        /// </summary>
        public LeftPanelUserControl()
        {
            InitializeComponent();
            viewModel = new MainViewModel();
            DataContext = viewModel;
            // Converter is now declared in XAML resources; no runtime registration needed.
        }

        private void AddNewStudentButton_Click(object sender, RoutedEventArgs e)
        {
            _addStudentView = new AddStudentView(viewModel);
            var parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                _addStudentView.Owner = parentWindow;
            }
            _addStudentView.ShowDialog();
        }

        private void SelectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var student in viewModel.Students)
            {
                student.IsSelected = true;
            }
        }

        private void SelectAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var student in viewModel.Students)
            {
                student.IsSelected = false;
            }
        }

        private void DeleteStudentButton_Click(object sender, RoutedEventArgs e)
        {
            var deleteConfirmationView = new DeleteConfirmationView(viewModel.Students);
            var parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                deleteConfirmationView.Owner = parentWindow;
            }
            deleteConfirmationView.ShowDialog();
        }

        private void EditStudentButton_Click(object sender, RoutedEventArgs e)
        {
            if(viewModel.SelectedStudent != null)
            {
                var editStudentView = new EditStudentView(viewModel.SelectedStudent, viewModel);
                var parentWindow = Window.GetWindow(this);
                if (parentWindow != null)
                {
                    editStudentView.Owner = parentWindow;
                }
                editStudentView.ShowDialog();
            }   
        }
    }

    // Converter to scale mark (0-100) to bar height (max 200 px)
    public class MarkToHeightConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int mark)
            {
                if (mark < 0) mark = 0;
                if (mark > 100) mark = 100;
                // scale to max 200px height
                return (double)mark * 2.0;
            }
            return 0.0;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
            => throw new System.NotSupportedException();
    }
}
