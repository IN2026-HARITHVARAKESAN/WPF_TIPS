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
}
